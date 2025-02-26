import React from "react";
import { useState } from "react";
import { useRef } from "react";
import InterviewPrep from "./InterviewPrep";
function InterviewPrepUseRefDemo() {
    const inputText = useRef(null);
    const btnClicktimes = useRef(0);
    const [ClickCount, setClickCount] = useState(0);

    const focusInputText = () => {
        inputText.current.focus();
    }

    const handleButtonClick = () => {
        btnClicktimes.current += 1;
        console.log(`Button clicked ${btnClicktimes.current} times a/c to ref`);
    }

    console.log("Acr ejwj rjejerfj fjrekfjrw wwjwie");
    const handleUpdateUi = () => {
        setClickCount(btnClicktimes.current);
    }
    
    return (
        <>
            <div>
                <input ref={inputText} type="text" placeholder="Write here !!!"></input>
                <button onClick={focusInputText}>Focus on the input text</button>
                <button onClick={handleButtonClick}>Btn Click</button>
                <button onClick={handleUpdateUi}>Update UI with btn clicks</button>
            </div>
        </>
    )
}

export default InterviewPrepUseRefDemo;