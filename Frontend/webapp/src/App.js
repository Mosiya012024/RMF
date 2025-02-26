import './App.css';
import Home from './components/Home';
import {BrowserRouter, Switch, Route} from "react-router-dom";
import React, { useEffect } from 'react';
import { Login } from "./components/Login";
import { SignUp } from './components/SignUp';
import { UserProvider } from './components/UserContext';
import TestFile from './components/TestFile';
import { RoomFinder } from './components/RoomFindex';
import RoomActualDetails from './components/RoomActualDetails';
import NavBar from './components/NavBar';
import store from './redux/store/store';
import { Provider } from 'react-redux';
import { PersistGate } from 'redux-persist/integration/react';
import { persistor } from './redux/store/store';
import LoginCopy from './components/LoginCopy';
import MyProfile from './components/MyProfile';
import ErrorBoundary from './shared/ErrorBoundary';
//import { InitializeSignalR } from './shared/NotificationService';
import ChatRoom from './components/ChatRoom';
import signalRServiceInstance from './shared/NotificationService2';
import ChatRoomCopy from './components/ChatRoomCopy';
import ChatNotifications from './components/ChatNotifications';
import ConfirmEmail from './ConfirmEmail';
import LoginMFACopy from './components/LoginMFACopy';
import LoginMFAQrCode from './components/LoginCopyMFAQrCode';
import InterviewPrep from './components/InterviewPrep';
import InterviewPrep2 from './components/InterviewPrep2';
import store2, { persistor2 } from './redux/store/store2';
import InterviewPrep4 from './components/InterviewPrep4';
import InterviewPrep5 from './components/InterviewPrep5';
import IntervirewPrep6 from './components/InterviewPrep6';
import InterviewPrepUseRefDemo from './components/InterviewPrepUseRefDemo';
import InterviewPrep7RenderProps from './components/InterviewPrep7RenderProps';
import InterviewPrepRenderPropsMain from './components/InterviewPrepRenderPropsMain';
import InterviewPrepReactMemo from './components/InterviewPrepReactMemo';
import InterviewPrepPureComponent from './components/InterviewPrepPureComponent';
import Parent from './Parent';
import { ContextProv } from './ContextHook';


function App() {
    const renderComponent = (component) => {
        return(<>
        <NavBar></NavBar>
        {component}
        </>)
    }

    useEffect(() => {

        console.log("App js is called");
        signalRServiceInstance.initializeSignalR();
        //InitializeSignalR();
        
    }, []);
    

    return (
        <React.Fragment>
            <Provider store={store2}>
                <PersistGate loading={null} persistor={persistor2}>
                <BrowserRouter>
                {/* <UserProvider> */}
                <ContextProv>
                <div className="App">
                    <Switch>
                        <Route exact path = "/home">
                        <Home></Home>
                        </Route>
                        {/* <Route exact path = "/log-in">
                            
                            <LoginCopy></LoginCopy>
                        </Route> */}

                        {/* <Route exact path = "/log-in">
                            <LoginMFACopy></LoginMFACopy>
                        </Route> */}
                        <Route exact path = "/log-in">
                            <LoginMFAQrCode></LoginMFAQrCode>
                        </Route>
                        <Route exact path = "/sign-up">
                            <SignUp></SignUp>
                        </Route>
                        {/* <Route exact path = "/sign-up" render={<ChatRoom></ChatRoom>}/> */}
                            
                        
                        <Route exact path = "/my-profile"
                            render={()=>renderComponent(<ErrorBoundary><MyProfile/></ErrorBoundary>)}
                        />
                        <Route exact 
                            path = "/places"
                            render={()=>
                            renderComponent(<RoomFinder/>)}
                        />
                        <Route exact 
                            path = "/:abcdef/:RoomDescriptionId"
                            render={()=>
                            renderComponent(<RoomActualDetails/>)}
                        />
                        {/* <Route exact 
                            path = "/:identity/chat/:RoomDescriptionId"
                            render = {()=>
                                renderComponent(<ChatRoom/>)}
                        /> */}
                        <Route exact 
                            path = "/chat-with/user/:toUserName"
                            render = {()=>
                                renderComponent(<ChatRoomCopy/>)}
                        />
                        <Route exact 
                            path = "/chat"
                            render = {()=>
                                renderComponent(<ChatNotifications/>)}
                        />
                        <Route exact 
                            path = "/confirm-email/email/token/:tokenID"
                            render = {() => 
                                renderComponent(<ConfirmEmail/>)
                            }
                        />
                            {/* <Route exact path = "/toast">
                                <RoomFinder></RoomFinder>
                            </Route>
                            
                            <Route exact path = "/toastData" >
                                <RoomActualDetails></RoomActualDetails>
                            </Route> */}
                        
                        <Route exact path = "/test">
                            <TestFile></TestFile>
                        </Route>
                        <Route exact path = "/intelliflo/interview/prep/react">
                            <InterviewPrep></InterviewPrep>
                        </Route>
                        <Route exact path = "/intelliflo">
                            <InterviewPrep2></InterviewPrep2>
                        </Route>
                        <Route exact path = "/intelliflo-ka-dhakkan">
                            <InterviewPrep2></InterviewPrep2>
                        </Route>
                        <Route exact path = "/reduce-func-demo">
                            <InterviewPrep4></InterviewPrep4>
                        </Route>
                        <Route exact path = "/useReducer-Hook-demo">
                            <InterviewPrep5></InterviewPrep5>
                        </Route>
                        <Route exact path = "/useReducer-Hook-complex-demo">
                            <IntervirewPrep6></IntervirewPrep6>
                        </Route>
                        <Route exact path = "/useRef-demo">
                            <InterviewPrepUseRefDemo></InterviewPrepUseRefDemo>
                        </Route>
                        <Route exact path = "/renderProps">
                            <InterviewPrepRenderPropsMain></InterviewPrepRenderPropsMain>
                        </Route>
                        <Route exact path = "/react-memo">
                            <InterviewPrepReactMemo></InterviewPrepReactMemo>
                        </Route>
                        <Route exact path = "/react-pure-component">
                            <InterviewPrepPureComponent></InterviewPrepPureComponent>
                        </Route>
                        <Route exact path = "/boon-ai">
                            <Parent></Parent>
                        </Route>
                    </Switch>
                </div>
                {/* </UserProvider> */}
                </ContextProv>
                </BrowserRouter>
            </PersistGate>
          </Provider>
      </React.Fragment>
    );
}

export default App;
