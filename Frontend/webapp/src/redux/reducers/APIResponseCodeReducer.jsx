const initialState = {
    ApiResponseCode: 0,
}

    
export const APIResponseCodeReducer = (state=initialState,action) => {
    console.log(action);
    switch(action.type) {
        case 'API RESPONSE CODE': 
            let newState = {...state, ApiResponseCode: action.ApiResponseCode};
            console.log(newState)
            return newState;
        default: 
            return state;
    }
}