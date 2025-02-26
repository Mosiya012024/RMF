import React, { ErrorInfo, ReactNode } from "react";
import { Component } from "react";
import { rootReducer } from "../redux/store/store";
import { connect } from "react-redux";


class ErrorBoundary extends Component {
    constructor(props) {
        super(props);
        this.state = {
            hasError: false,
        }
    }

    static getDerivedStateFromError() {
        return { hasError: true };
    }

    componentDidCatch(error, errorInfo) {
        console.error("Error caught by Error Boundary:", error, errorInfo);
    }

    redirectToHome = () => {
        window.open('/', '_self');
    }

    render() {
        console.log(this.state.hasError);
        console.log(this.props.apiResponseCode)
        //Object.keys(this.props.currentUserInfo).length === 0
        if(this.state.hasError || this.props.apiResponseCode === 401) {
            return (
                <>
                 <div>Hello error coming up here</div>
                 <div>Hello error coming up here</div>
                 <div>Hello error coming up here</div>
                 <div>Hello error coming up here</div>
                 <div>Hello error coming up here</div>
                 <div>Hello error coming up here</div><div>Hello error coming up here</div>
                 <div>Hello error coming up here</div>
                 <div>Hello error coming up here</div>
                 <div>Hello error coming up here</div><div>Hello error coming up here</div>
                 <div>Hello error coming up here</div>
                 <div>Hello error coming up here</div>
                 <div>Hello error coming up here</div><div>Hello error coming up here</div>
                 <div>Hello error coming up here</div>
                 <div>Hello error coming up here</div><div>Hello error coming up here</div>
                 <div>Hello error coming up here</div>
                 <div>Hello error coming up here</div>

                 <button style={{width:'50px',height:'50px',color:'greenyellow'}} onClick={()=>this.redirectToHome()}>Back to Home</button>
                </>
            )
        }
        return this.props.children;
        
    }
}

const mapStateToProps = (state=rootReducer) => ({
    apiResponseCode: state.APIResponseCode.ApiResponseCode,
    currentUserInfo: state.CurrentUserInfo.CurrentUserInfo
})
export default connect(mapStateToProps)(ErrorBoundary);