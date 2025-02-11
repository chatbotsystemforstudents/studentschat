export class Speech {
    audioUrl: string;
    mouthCuesUrl: string;

    constructor(audioUrl: string, mouthCuesUrl:string) {
        this.audioUrl = audioUrl;
        this.mouthCuesUrl = mouthCuesUrl;
    }
}