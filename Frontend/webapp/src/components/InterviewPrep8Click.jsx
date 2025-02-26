import { Component } from "react";

class InterviewPrep8Click extends Component {
    constructor(props) {
        super(props);
        this.state = {

        }
    }
    render() {
        const {count, handleCount} = this.props
        return (
            <div onClick={handleCount}>Click Count - {count}</div>
        )
    }
}
export default InterviewPrep8Click;