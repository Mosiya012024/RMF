import { Component } from "react";

class InterviewPrep9Hover extends Component {
    constructor(props) {
        super(props);
        this.state = {

        }
    }
    render() {
        const {count, handleCount} = this.props
        return (
            <div onClick={handleCount}>Hover Count - {count}</div>
        )
    }
}
export default InterviewPrep9Hover;