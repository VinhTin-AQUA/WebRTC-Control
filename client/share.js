let connection = null;
let peerConnection = null;
let localStream = null;
let receivedChannel = null;

async function ensureConnection() {
	if (!connection) {
		connection = new signalR.HubConnectionBuilder()
			.withUrl('https://localhost:7072/screensharehub')
			.build();

		connection.on('ReceiveSignal', async (user, data) => {
			await handleSignal(data);
		});

		await connection.start().catch(err => console.error('Connection failed:', err));
	} else if (connection.state !== signalR.HubConnectionState.Connected) {
		await connection.start().catch(err => console.error('Connection failed:', err));
	}
}

async function createPeer() {
	if (peerConnection) return;

	peerConnection = new RTCPeerConnection({
		iceServers: [{ urls: 'stun:stun.l.google.com:19302' }],
	});

	// Debug connection states
	peerConnection.oniceconnectionstatechange = () => {
		console.log('ICE Connection State:', peerConnection.iceConnectionState);
	};

	peerConnection.onconnectionstatechange = () => {
		console.log('Connection State:', peerConnection.connectionState);
	};

	peerConnection.onicecandidate = event => {
		if (event.candidate) {
			connection.invoke('SendSignal', '', event.candidate);
		}
	};

	peerConnection.ondatachannel = event => {
		console.log('DataChannel received!');
		receivedChannel = event.channel;

		receivedChannel.onopen = () => {
			console.log('DataChannel opened on Sharer side');
			receivedChannel.send('Hello from Sharer!');
		};

		receivedChannel.onmessage = event => {
			try {
				handleData(JSON.parse(event.data));
			} catch (ex) {
				console.log(ex);
				console.log(event.data);
			}
		};

		receivedChannel.onclose = () => {
			console.log('DataChannel closed on Sharer side');
		};

		receivedChannel.onerror = error => {
			console.error('DataChannel error:', error);
		};
	};
}

async function startShare() {
	await ensureConnection();
	await createPeer();
	await sendDemensionOfWindow();

	try {
		localStream = await navigator.mediaDevices.getDisplayMedia({
			video: true,
			audio: false,
		});

		localStream.getTracks().forEach(track => {
			peerConnection.addTrack(track, localStream);
		});

		const offer = await peerConnection.createOffer();
		await peerConnection.setLocalDescription(offer);
		await connection.invoke('SendSignal', '', offer);

		document.getElementById('btnStartShare').disabled = true;
		document.getElementById('btnStopShare').disabled = false;
	} catch (err) {
		console.error('Could not start screen share:', err);
	}
}

async function stopShare() {
	if (localStream) {
		localStream.getTracks().forEach(track => track.stop());
		localStream = null;
	}

	if (peerConnection) {
		peerConnection.close();
		peerConnection = null;
	}

	document.getElementById('btnStartShare').disabled = false;
	document.getElementById('btnStopShare').disabled = true;
}

async function handleSignal(data) {
	if (!peerConnection) await createPeer();

	if (data.type === 'offer') {
		await peerConnection.setRemoteDescription(new RTCSessionDescription(data));
		const answer = await peerConnection.createAnswer();
		await peerConnection.setLocalDescription(answer);
		await connection.invoke('SendSignal', '', answer);
	} else if (data.candidate) {
		try {
			await peerConnection.addIceCandidate(new RTCIceCandidate(data));
		} catch (e) {
			console.error('Error adding ICE candidate:', e);
		}
	}
}

async function sendData(data) {
	receivedChannel.send(JSON.stringify(data));
}

function DOMContentLoadedEvent() {
	window.addEventListener('DOMContentLoaded', () => {
		ensureConnection().catch(err => console.error('Connection error:', err));
	});
}

function clickApp() {
	document.addEventListener('click', event => {
		// Tọa độ click trên thẻ video (Operator)
		const operatorX = event.screenX;
		const operatorY = event.screenY;
		console.log(`Tọa độ thực trên Client: (${operatorX}, ${operatorY})`);
	});
}

DOMContentLoadedEvent();
clickApp();

/* ============================================== */

const DataType = Object.freeze({
	INIT_SCALE: 'INIT_SCALE',
	LEFT_MOUSE_CLICK_EVENT: 'LEFT_MOUSE_CLICK_EVENT',
	RIGHT_MOUSE_CLICK_EVENT: 'RIGHT_MOUSE_CLICK_EVENT',
	MOUSE_SCROLL_EVENT: 'MOUSE_SCROLL_EVENT',
});

const appControlUrl = 'http://localhost:5000';

function handleData(data) {
	switch (data.type) {
		case DataType.LEFT_MOUSE_CLICK_EVENT:
			sendLeftMouseClickToService(data);
			break;
		case DataType.RIGHT_MOUSE_CLICK_EVENT:
			sendRightMouseClickToService(data);
			break;
		case DataType.MOUSE_SCROLL_EVENT:
			sendMouseScrollToService(data);
			break;
		default:
			break;
	}
}

async function sendDemensionOfWindow() {
	const data = {
		type: DataType.INIT_SCALE,
		screenWidth: screen.width,
		screenHeight: screen.height,
	};
	await sendData(data);
}

// hàm click trên trình duyệt
// function remoteClick(x, y) {
// 	// document.elementFromPoint(x, y).click();
// 	const ev = new MouseEvent('click', {
// 		view: window,
// 		bubbles: true,
// 		cancelable: true,
// 		screenX: x,
// 		screenY: y,
// 	});
// 	const el = document.elementFromPoint(x, y);
// 	el.dispatchEvent(ev);
// }

/* gửi mouse event tới dịch vụ thực hiện tương tác */
function sendLeftMouseClickToService(data) {
	console.log(data.scaleX, data.scaleY);

	fetch(appControlUrl + '/handle-left-click', {
		method: 'POST',
		headers: { 'Content-Type': 'application/json' },
		body: JSON.stringify({ x: data.scaleX, y: data.scaleY }),
	})
		.then(res => res.text())
		.then(msg => console.log(msg));
}

function sendRightMouseClickToService(data) {
	console.log(data.scaleX, data.scaleY);

	fetch(appControlUrl + '/handle-right-click', {
		method: 'POST',
		headers: { 'Content-Type': 'application/json' },
		body: JSON.stringify({ x: data.scaleX, y: data.scaleY }),
	})
		.then(res => res.text())
		.then(msg => console.log(msg));
}

function sendMouseScrollToService(data) {
	console.log(data.deltaX, data.deltaY);

	fetch(appControlUrl + '/handle-mouse-scroll', {
		method: 'POST',
		headers: { 'Content-Type': 'application/json' },
		body: JSON.stringify({ deltaX: data.deltaX, deltaY: data.deltaY }),
	})
		.then(res => res.text())
		.then(msg => console.log(msg));
}
