import { useReducer } from "react"

//some complex scenario of useReducer taking state as an object.
function InterviewPrep6() {

    const initialState = {
        firstCounter: 0
    }

    const reducerCallBackFunc = (curr_State, action) => {
        switch(action.type) {
            case 'increment' : return {...curr_State, firstCounter: curr_State.firstCounter+action.value};
            case 'decrement' : return {...curr_State, firstCounter: curr_State.firstCounter-action.value};
            case 'Reset' : return curr_State;
            default: 
                return curr_State;
        }


    }
    const [newState, dispatch] = useReducer(reducerCallBackFunc, initialState);
    return (
        <div>
            <div>Count - {newState}</div>
            <button onClick={()=>dispatch({type: 'increment', value: 1})}>Increment1</button>
            <button onClick={()=>dispatch({type: 'decrement', value: 1})}>Decrement1</button>
            <button onClick={()=>dispatch({type: 'reset', value: 1})}>Reset</button>
            <button onClick={()=>dispatch({type: 'increment', value: 5})}>Increment5</button>
            <button onClick={()=>dispatch({type: 'decrement', value: 5})}>Decrement5</button>
           
        </div>
    )
}

export default InterviewPrep6;