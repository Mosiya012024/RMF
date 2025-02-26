import { useReducer } from "react";

function InterviewPrep5() {

    const initialState = 0;

    const reducerCallBackFunc = (curr_State, action) => {
        switch(action) {
            case 'Increment' : return curr_State+1;
            case 'Decrement' : return curr_State-1;
            case 'Reset' : return curr_State;
            default: 
                return curr_State;
        }
    }
    const [newState,dispatch] = useReducer(reducerCallBackFunc, initialState);

    
    return (
        <div>
            <div>Count - {newState}</div>
            <button onClick={()=>dispatch('Increment')}>Increment</button>
            <button onClick={()=>dispatch('Decrement')}>Decrement</button>
            <button onClick={()=>dispatch('Reset')}>Reset</button>
        </div>
    )
}
export default InterviewPrep5;