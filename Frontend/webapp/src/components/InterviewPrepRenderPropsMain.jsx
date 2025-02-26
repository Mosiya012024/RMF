import { Component } from "react";
import InterviewPrep7RenderProps from "./InterviewPrep7RenderProps";
import InterviewPrep8Click from "./InterviewPrep8Click";
import InterviewPrep9Hover from "./InterviewPrep9Hover";

class InterviewPrepRenderPropsMain extends Component {
    render() {
        return(
            <>
                <InterviewPrep7RenderProps
                    render={(count,handleCount)=>(<InterviewPrep8Click count={count} handleCount={handleCount}/>)}
                />
                <InterviewPrep7RenderProps
                    render={(count,handleCount)=>(<InterviewPrep9Hover count={count} handleCount={handleCount}/>)}
                />
            </>
        )
    }
}

export default InterviewPrepRenderPropsMain;