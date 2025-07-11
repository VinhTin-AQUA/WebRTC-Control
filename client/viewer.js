let connection = null;
let peerConnection = null;
let dataChannel = null;

const DataType = Object.freeze({
	INIT_SCALE: 'INIT_SCALE',
	LEFT_MOUSE_CLICK_EVENT: 'LEFT_MOUSE_CLICK_EVENT',
	RIGHT_MOUSE_CLICK_EVENT: 'RIGHT_MOUSE_CLICK_EVENT',
});

let scaleX = 0;
let scaleY = 0;

let sharerWith = 0;
let sharerHeight = 0;

const videoElement = document.getElementById('remoteVideo');

/* ====================================================== */

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

	peerConnection.ontrack = event => {
		console.log('Received remote stream');
		document.getElementById('remoteVideo').srcObject = event.streams[0];
	};
}

async function initDataChannel() {
	dataChannel = peerConnection.createDataChannel('remote-control', {
		ordered: true,
		maxRetransmits: 3,
	});

	dataChannel.onopen = () => {
		console.log('DataChannel opened!');
		// Test send message
		dataChannel.send('Hello from Viewer!');
	};

	dataChannel.onmessage = event => {
		try {
			handleData(JSON.parse(event.data));
		} catch {
			console.log(event.data);
		}
	};

	dataChannel.onerror = error => {
		console.error('DataChannel error:', error);
	};

	dataChannel.onclose = () => {
		console.log('DataChannel closed');
	};
}

async function startViewer() {
	await ensureConnection();
	await createPeer();
	await initDataChannel();
	calcScaleScreen();

	const offer = await peerConnection.createOffer();
	await peerConnection.setLocalDescription(offer);
	await connection.invoke('SendSignal', '', offer);
}

async function handleSignal(data) {
	// console.log(data);

	if (!peerConnection) await createPeer();

	if (data.type === 'offer') {
		await peerConnection.setRemoteDescription(new RTCSessionDescription(data));
		const answer = await peerConnection.createAnswer();
		await peerConnection.setLocalDescription(answer);
		await connection.invoke('SendSignal', '', answer);
	} else if (data.type === 'answer') {
		await peerConnection.setRemoteDescription(new RTCSessionDescription(data));
	} else if (data.candidate) {
		try {
			await peerConnection.addIceCandidate(new RTCIceCandidate(data));
		} catch (e) {
			console.error('Error adding ICE candidate:', e);
		}
	} else if (data.type === 'viewer-ready' && isSharing) {
		// Viewer is ready, re-send offer
		const offer = await peerConnection.createOffer();
		await peerConnection.setLocalDescription(offer);
		await connection.invoke('SendSignal', '', offer);
	}
}

async function sendData(data) {
	dataChannel.send(JSON.stringify(data));
}

function DOMContentLoadedEvent() {
	window.addEventListener('DOMContentLoaded', () => {
		startViewer().catch(err => console.error('Viewer error:', err));
	});
}

// bắt sự kiện chuột (click chuột trái, phải, scroll)
function remoteVideoEvent() {
	videoElement.addEventListener('mousedown', event => {
		const operatorX = event.offsetX;
		const operatorY = event.offsetY;
		// console.log(`Tọa độ thực trên Client: (${operatorX}, ${operatorY})`);
		sendMouseClickEvent(operatorX, operatorY, event.button);
	});

	videoElement.addEventListener('contextmenu', event => {
		event.preventDefault(); // Ngăn hiển thị menu chuột phải
	});
}

DOMContentLoadedEvent();
remoteVideoEvent();

/* ====================================================== */

function initScaler(clientWidth, clientHeight) {
	sharerWith = clientWidth;
	sharerHeight = clientHeight;
}

function calcScaleScreen() {
	const timeoutId = setTimeout(() => {
		scaleX = videoElement.clientWidth / sharerWith;
		scaleY = videoElement.clientHeight / sharerHeight;
	}, 3000);
}

function handleData(data) {
	switch (data.type) {
		case DataType.INIT_SCALE:
			initScaler(data.screenWidth, data.screenHeight);
			break;
	}
}

/* mouse event */
function sendMouseClickEvent(viewerX, viewerY, mouseType) {
	const remoteX = viewerX / scaleX;
	const remoteY = viewerY / scaleY;

	console.log(remoteX);
	console.log(remoteY);

	// clearTimeout(timeoutId);
	const data = {
		type:
			mouseType == 0
				? DataType.LEFT_MOUSE_CLICK_EVENT
				: mouseType == 2
				? DataType.RIGHT_MOUSE_CLICK_EVENT
				: 2,
		scaleX: Math.round(remoteX),
		scaleY: Math.round(remoteY),
	};

	sendData(data)
		.then()
		.catch(err => console.log(err));
}
