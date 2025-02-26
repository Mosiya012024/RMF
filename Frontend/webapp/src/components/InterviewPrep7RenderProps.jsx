import { Component } from "react";
import { UserContext } from "./UserContext";

class InterviewPrep7RenderProps extends Component {
    constructor(props) {
        super(props);
        this.state = {
            count: 0,
        }
    }

    static contextType = UserContext;

    handleCount = () => {
        this.setState(prevState=>{
            return {count: prevState.count+1}
        });
    }

    render() {
        console.log(contextType);
        return (
            <div>
                {this.props.render(this.state.count,this.handleCount)}
            </div>
        )
    }
}
export default InterviewPrep7RenderProps;