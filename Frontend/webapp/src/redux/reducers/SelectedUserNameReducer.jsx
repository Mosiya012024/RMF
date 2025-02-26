const initialState = {
    SelectedUserName: '',
}

    
export const SelectedUserNameReducer = (state=initialState,action) => {
    console.log(action);
    switch(action.type) {
        case "SELECTED USER NAME": 
            let newState = {...state, SelectedUserName: action.SelectedUserName};
            return newState;
        default: 
            return state;
    }
}