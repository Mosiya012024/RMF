const initialState = {
    SelectedUserType : ""
}
export const SelectedUserTypeReducer = (state = initialState, action) => {
    switch(action.type) {
        case 'SELECTED USER TYPE': 
            let newState = {...state, SelectedUserType: action.SelectedUserType};
            return newState;
        default: 
            return state;
    }
}