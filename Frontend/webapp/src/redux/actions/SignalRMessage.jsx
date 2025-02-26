export const setSignalRMessage = (SignalRMessage) => {
    return {
        type: 'SIGNALR MESSAGE',
        SignalRMessage: SignalRMessage, 
    }

}