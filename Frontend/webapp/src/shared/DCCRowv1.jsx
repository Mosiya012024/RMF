import { Menu } from "@mui/material";
import MenuItem from "@mui/material/MenuItem";
import React from "react";
import { Component } from "react";

class DCCRowv1 extends Component {
    constructor(props) {
        super(props);
        this.state = {
            
        }
    }

    handleCloseMenu = () => {
        this.setState({open:this.state.open});
    }

    handleClickMenuItem = () => {
        this.setState({open:false});
    }

    render() {
        console.log(this.state.open);
        return(
        
                <Menu
                    id="basic-menu"
                    open={this.props.open}
                    onClose={this.handleCloseMenu}
                    MenuListProps={{
                    'aria-labelledby': 'basic-button',
                    sx: {
                        
                    }
                    }}
                >
                    <MenuItem onClick={()=>this.handleClickMenuItem()}>Profile</MenuItem>
                    <MenuItem onClick={()=>this.handleClickMenuItem()}>My account</MenuItem>
                    <MenuItem onClick={()=>this.handleClickMenuItem()}>Logout</MenuItem>
                </Menu>
            
        )
    }
}

export default DCCRowv1