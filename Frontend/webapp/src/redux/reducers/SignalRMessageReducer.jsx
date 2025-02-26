const initialState = {
    SignalRMessage: {}
}

export const SignalRMessageReducer = (state = initialState,action) => {
    switch (action.type) {
        case 'SIGNALR MESSAGE' :
            let newState = {...state};
            newState = {...state, SignalRMessage: action.SignalRMessage};
            return newState;
        default: 
            return state;
    }
}