import { useDispatch } from "react-redux"
import { setCheckCode } from "../redux/actions/CheckAction";
import store2 from "../redux/store/store2";

function InterviewPrep3 (props) {

    const dispatch = useDispatch();

    const handleSendss = () => {
        //dispatch(setCheckCode(33333));
        store2.dispatch(setCheckCode(3333));
        props.handleClick("Hii I am child component, I am sending data to you");
    }
    return (
        <>
        <div>
            <button onClick={handleSendss}>Interviw Prep 3</button>
        </div>
        </>
    )
}
export default InterviewPrep3