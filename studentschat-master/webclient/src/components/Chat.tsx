import { useState, useRef, useEffect } from 'react'
//import teacherAvatar from '../assets/teacher-avatar.jpeg';
import { Canvas, useThree } from "@react-three/fiber";
import { Environment, useTexture } from "@react-three/drei";
import Avatar from './Avatar';
import { SendToApi,PollUrl } from '../services/HttpClient';
import { Speech } from '../models/Shared';
enum InteractionState {
	ReadyToStartListening = 0,
	Listening = 1,
	WaitingForResponse = 2,
	ResponseHasBeenReceived = 3,
	ResponseHasBeenPlayed = 4
}
interface AvatarSceneProps {
	speech:Speech
}
function AvatarScene({ speech }: AvatarSceneProps) {
	const texture = useTexture('/textures/classroom.jpeg');
	const { viewport } = useThree();
	const aspect = texture.image.width / texture.image.height;
	const planeWidth = viewport.width;
	const planeHeight = planeWidth / aspect;

	return (
		<>
			<color attach="background" args={["#ececec"]} />
			<Avatar groupProps={{ position: [0, -3, 5], scale: 2 }} speech={speech} />
			<Environment preset="sunset" />
			<mesh>
				<planeGeometry args={[planeWidth, planeHeight]} />
				<meshBasicMaterial map={texture} />
			</mesh>
		</>
	);
}
function Chat() {
	var webApiUrl = import.meta.env.VITE_API_URL;
	var speechResponsesAudioFilesUrl = import.meta.env.VITE_SPEECH_RESPONSES_URL;
	const [showForm, setShowForm] = useState(false);
	const [fileName, setFileName] = useState('');
	const [transcript, setTranscript] = useState('');
	//const [error, setError] = useState<string | null>(null);
	const [speech, setSpeech] = useState<Speech | null>(null);
	const [interimTranscript, setInterimTranscript] = useState('');
	//const [lastPlayedFileName, setLastPlayedFileName] = useState('');
	const [interactionState, setInteractionState] = useState(InteractionState.ReadyToStartListening);
	let recognition: any;
	const interactionStateRef = useRef(interactionState);
	
	useEffect(() => {
		interactionStateRef.current = interactionState;
	}, [interactionState]);
	useEffect(() => {
		if (interactionStateRef.current == InteractionState.WaitingForResponse && fileName !== '') {
			const speechUrl = `${speechResponsesAudioFilesUrl}/${fileName}.wav`;
			const lipSyncUrl = `${speechResponsesAudioFilesUrl}/${fileName}.json`;
			PollUrl(speechUrl)
				.then(async () => {
					console.log("speech received for " + fileName);
					await SendToApi({ Guid: fileName }, `${webApiUrl}/GenerateLipSync`);
					return PollUrl(lipSyncUrl);
				})
				.then(() => {
					console.log("lipsync received for " + fileName);
					setInteractionState(InteractionState.ResponseHasBeenReceived); // URL exists and returns a 200
					setSpeech(new Speech(speechUrl, lipSyncUrl));
					//setLastPlayedFileName(fileName);
					setInteractionState(InteractionState.ReadyToStartListening);
				})
				.catch((error) => {
					setInteractionState(InteractionState.ReadyToStartListening);
					console.error("Polling error:", error);
				});
			return () => {}
		}

	}, [interactionState, fileName]);

	const audioRef = useRef<HTMLAudioElement | null>(null);
	const playAudio = (audioPath: string, onEndcallback: (() => void) | null = null) => {
		if (!audioRef.current) {
			audioRef.current = new Audio(audioPath);
		}
		audioRef.current.play();
		audioRef.current.onended = onEndcallback;
	};
	

	if ('webkitSpeechRecognition' in window || 'SpeechRecognition' in window) {
		const SpeechRecognition =
			(window as any).SpeechRecognition || (window as any).webkitSpeechRecognition;
		recognition = new SpeechRecognition();
		recognition.continuous = true;
		recognition.interimResults = true;
		recognition.lang = 'el-GR';

		recognition.onresult = (event: any) => {
			if (interactionStateRef.current == InteractionState.Listening) {
				var _interimTranscript = '';
				for (let i = event.resultIndex; i < event.results.length; i++) {
					const transcriptPart = event.results[i][0].transcript;
					_interimTranscript += transcriptPart;
					console.log(_interimTranscript);
					//if (event.results[i].isFinal) {
					//	setTranscript((prev) => prev + transcriptPart);
					//} else {						
					//	_interimTranscript += transcriptPart;						
					//}
				}
				setInterimTranscript(_interimTranscript);
			}
		};

		recognition.onerror = (event: any) => {
			console.error(`Error occurred: ${event.error}`);
		};
	} else {
		console.error('Speech Recognition API is not supported in this browser.');
	}
	const playIntroSound = () => {
		playAudio(`${speechResponsesAudioFilesUrl}/init-mic-white-noise.mp3`, () => {
			setInteractionState(InteractionState.ReadyToStartListening);
			setShowForm(true);
		});
	}
	const startListening = () => {
		setTranscript("");
		setInteractionState(InteractionState.Listening);
		recognition.start();
	};

	const stopListening = () => {
		setTranscript(interimTranscript);
		setInteractionState(InteractionState.WaitingForResponse);
		recognition.stop();
		SendToApi({ text: interimTranscript }, `${webApiUrl}/GenerateSpeech`)
			.then((data) => {
				setFileName((data as any).fileName);
				console.log('Response from API:', data);
			})
			.catch((error) => {
				console.error("Error:", error);
			});
	};	
	return (
		<>
			<div>
				<button onClick={() => playIntroSound()} style={{ display: !showForm ? 'block' : 'none' }}>
					Εναρξη συνομιλίας
				</button>
				<div style={{ display: showForm ? 'block' : 'none' }}>
					<div style={{
						width: '95vw',
						height: '80vh',
						display: 'flex',
						justifyContent: 'center',
						alignItems: 'center',
						margin: '0', // Remove any default margins
						overflow: 'hidden' // Prevent scrollbars
					}}>
						<div style={{
							width: '95%',
							height: '95%',
							border: '1px solid black',
							display: 'flex', // Optional: Centers content inside this div if needed
							justifyContent: 'center',
							alignItems: 'center'
						}}>
							<Canvas shadows camera={{ position: [0, 0, 8], fov: 60 }}>
								<AvatarScene speech={speech!} />
							</Canvas>
						</div>
					</div>					
					{/*{error && <p style={{ color: 'red' }}>{error}</p>}*/}
					<button onClick={startListening} disabled={interactionState != InteractionState.ReadyToStartListening}>
						Start Listening
					</button>
					<button onClick={stopListening} disabled={interactionState != InteractionState.Listening}>
						Stop Listening
					</button>
					<h2>Transcript:</h2>
					<p>{transcript}</p>
				</div>
			</div>
		</>
	)
}

export default Chat
