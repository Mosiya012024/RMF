const initialState = {
    CurrentUserInfo : {},
}
export const CurrentUserInfoReducer = (state=initialState,action) => {
    switch(action.type) {
        case 'CURRENT USER INFO': 
            let newState = {...state,CurrentUserInfo: action.CurrentUserInfo};
            return newState;
        default:
            return state;
    }
}