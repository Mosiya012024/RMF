import { useEffect }from "react";
import React from "react";
import { toast } from "react-toastify";
import { ToastContainer } from "react-toastify";
import "react-toastify/dist/ReactToastify.css";
import { UserContext } from "./UserContext";
import { useContext , useState } from "react";
import RMFLogo from "../images/roommatefinder_logo_final.png";
import "./NavBar.css"
import CreateRoomwithProgress from "./CreateRoomwithProgress";
import PostRoomDetails from "./PostRoomDetails";
import userLogo from '../images/girl-image.jpg'
import Box from '@mui/material/Box';
import Drawer from '@mui/material/Drawer';
import Button from '@mui/material/Button';
import List from '@mui/material/List';
import Divider from '@mui/material/Divider';
import ListItem from '@mui/material/ListItem';
import ListItemButton from '@mui/material/ListItemButton';
import ListItemIcon from '@mui/material/ListItemIcon';
import ListItemText from '@mui/material/ListItemText';
import InboxIcon from '@mui/icons-material/MoveToInbox';
import MailIcon from '@mui/icons-material/Mail';
import { useHistory } from "react-router-dom/cjs/react-router-dom";
import store, { rootReducer } from "../redux/store/store";
import { connect } from "react-redux";
import { useSelector } from "react-redux";
import { setCurrentUserInfo } from "../redux/actions/CurrentUserInfo";
import { bindActionCreators } from "redux";
import { IconButton } from "@mui/material";
import NotificationsIcon from '@mui/icons-material/Notifications';


function NavBar(props) {
    const [[userData,setUserData],[firstLogin,setFirstLogin]] = useContext(UserContext);
    console.log(userData);
    console.log(firstLogin);
    const [makeUserProfileVisible, setUserProfileVisibility] = useState(false);
    const [OpenCreateRoomPopup, setCreateRoomPopup] = useState(false);
    const history = useHistory();
    

    const toggleDrawer = (newOpen) => () => {
        setUserProfileVisibility(newOpen);
    };
    const buttonText = useSelector(rootReducer => rootReducer.CurrentUserInfo.CurrentUserInfo.name);

    useEffect(()=>{
        if(firstLogin) {
            toast.success(`Welcome ${props.currentUserInfo.CurrentUserInfo.name}!`,{
                position: 'top-center',
                autoClose: 6000
            });
            setFirstLogin(false);
        }
    })

    const handleLogOutUser = () =>{
        setUserData({...userData,
            UserType : "",
            Name : "",
            Email : ""
        })
        props.setCurrentUserInfo({});
        localStorage.setItem('userToken',"");
    }

    const handleUserNameClick = () =>{
        setUserProfileVisibility(true);
    }
    
    const SideBar = () => {
        return (
            <button className="logout-btn" onClick={handleLogOutUser}>logout</button>
        )
    }
    const openUserProfile = makeUserProfileVisible && (<SideBar></SideBar>)
    const handlePostRoom = () => {
        setCreateRoomPopup(true);
    }

    const closeCreateRoomPopup = () => {
        setCreateRoomPopup(false);
    }
    
    let openCreateRoomPopUp = (
        // <CreateRoom open={OpenCreateRoomPopup} handleClose={closeCreateRoomPopup}>
        // </CreateRoom>
        // <CreateRoomwithProgress open={OpenCreateRoomPopup} handleClose={closeCreateRoomPopup}>
        // </CreateRoomwithProgress>
        <PostRoomDetails open={OpenCreateRoomPopup} handleClose={closeCreateRoomPopup}>
        </PostRoomDetails>
    ) 
    const DrawerListItems = (listItemText) => {
        if(listItemText === "My Profile") {
            history.push('my-profile');
        }
        if(listItemText === "Logout") {
            handleLogOutUser();
        }
        if(listItemText === "All Listings") {
            history.push("/places");
        }
        if(listItemText === "My Notifications") {
            history.push("/chat");
        }
    }

    const DrawerList = (
        <Box sx={{ width: 250 }} role="presentation" onClick={toggleDrawer(false)}>
          <List>
            {['All Listings','My Preferences', 'My Notifications', 'My Profile', 'Logout'].map((text, index) => (
              <ListItem key={text} disablePadding>
                <ListItemButton onClick={()=>DrawerListItems(text)}>
                    <ListItemText primary={text}/>
                        <ListItemIcon>
                            {text === "My Notifications" && (
                                <IconButton>
                                    <NotificationsIcon></NotificationsIcon>
                                </IconButton>
                                )}
                      </ListItemIcon>
                </ListItemButton>
              </ListItem>
            ))}
          </List>
        </Box>
      );
    
    const navigateToHome = () => {
        history.push('/Home');
    }
    
    return(
        <div>
            <div>
            <ToastContainer />
            </div>
            <div className="user-nav-bar">
                <div className="user-app-logo" onClick={navigateToHome}>
                    <img src = {RMFLogo} alt="Room Mate Finder logo" width={200} height={80}></img>
                </div>
                {/* <div onClick={handleUserNameClick} className="user-logo">
                    <span><img src={userLogo} alt="user logo" width={"50px"} height={'50px'}></img></span>
                    <span className="user-name">{userData.Name}</span>
                </div> */}
                {/* {openUserProfile} */}
                <div className="post-room-btn"onClick={handlePostRoom}>Add Listing</div>
                <div className="user-logo">
                    <Button onClick={toggleDrawer(true)}>
                        <div onClick={handleUserNameClick} style={{display:'flex',flexDirection:'row'}}>
                            <span><img src={userLogo} alt="user logo" width={"50px"} height={'50px'}></img></span>
                            <span className="user-name">{props.currentUserInfo.CurrentUserInfo.name}</span>
                            {/* <span className="user-name">{buttonText}</span> */}
                        </div>
                    </Button>
                    <Drawer open={makeUserProfileVisible} onClose={toggleDrawer(false)}>
                        {DrawerList}
                    </Drawer>
                </div>
                {openCreateRoomPopUp}
            </div>
        </div>);
}

const mapStateToProps = (state = rootReducer) => ({
    currentUserInfo: state.CurrentUserInfo,
});

const mapDispatchToProps = (dispatch) => ({
    setCurrentUserInfo: bindActionCreators(setCurrentUserInfo,dispatch),
})
//we can access the redux state in functional component using mapStateToProps and pass teh props in fucntional Component or we can directly use
//useSelector hook provided by react-redux and access a particular property of react state

export default connect(mapStateToProps,mapDispatchToProps)(NavBar);
//export default NavBar