const initialState = {
    CheckAction: 0
}

export const CheckReducer = (state=initialState, action) => {
    console.log(action)
    switch (action.type) {
        case 'CHECK_ACTION' : 
        let newState = {...state, CheckAction : action.CheckAction};
        console.log(newState);
        return newState;
        default: 
        return state;
    }
}