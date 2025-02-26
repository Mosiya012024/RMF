const initialState = {
    RefreshScreenData: {}
};
export const RefreshScreenDataReducer = (state = initialState,action ) => {
    switch(action.type) {
        case 'REFRESH SCREEN DATA' : 
            let newState = {...state,RefreshScreenData: action.RefreshScreenData};
            return newState;
        default: 
            return state;
    }
}