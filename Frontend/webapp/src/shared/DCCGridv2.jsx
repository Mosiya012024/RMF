import { TableContainer, TableHead, TableRow,TableCell, Table, TableBody, TableSortLabel } from "@mui/material";
import Box from '@mui/material/Box';
import { Component } from "react";
import DCCRowv1 from "./DCCRowv1";
import { Menu } from "@mui/material";
import MenuItem from "@mui/material/MenuItem";
import PostRoomDetails from "../components/PostRoomDetails";
import { rootReducer } from "../redux/store/store";
import { connect } from "react-redux";
import React from "react";
import { withRouter } from "react-router-dom/cjs/react-router-dom";



class DCCGridv2 extends Component {
    constructor(props) {
        super(props);
        
        this.state = {
            sortType: 'asc',
            sortedColumn: "",
            // sortedData: this.sortData(this.props.columns[0],'asc',""),
            sortedData: [],
            openMoreDetails: false,
            anchorElElement: null,
            openEditRoomPopup:false,
            roomDetails:{}
        }
        this.handleSortColumn = this.handleSortColumn.bind(this);
    }

    componentDidMount = () => {
        this.sortData(this.state.sortedColumn,this.state.sortType)
    }

    sortData = () => {
        console.log("Original rowsData:", this.props.rowsData);
        const sortedData = [...this.props.rowsData]; // Create a shallow copy
        console.log("Shallow copy before sorting:", sortedData);
        return sortedData;
    }
    
    
    handleSortColumn = (ColumnName) => {
        if(this.state.sortedColumn === ColumnName) {
            this.setState({sortType: this.state.sortType === 'asc' ? 'desc' : 'asc'},
                ()=>{this.props.setOrderByString(this.state.sortedColumn+"_"+this.state.sortType)}
            );
            return;
        }
        this.setState({sortedColumn: ColumnName,sortType: 'asc'},
            ()=>{this.props.setOrderByString(this.state.sortedColumn+"_"+this.state.sortType)}
        );
    }

    visibleRows = (column,type) => {
        return this.sortData();
    }

    handleMoreDetails = (event,rowData) => {
        this.setState({openMoreDetails: !this.state.openMoreDetails,anchorElElement:event,roomDetails:rowData});
    }

    handleCloseMenu = () => {
        this.setState({openMoreDetails: !this.state.openMoreDetails});
    }

    handleEditClickMenuItem = () => {
        this.setState({openMoreDetails: !this.state.openMoreDetails});
        this.props.openEditRoomPopup(this.state.roomDetails["id"],this.state.roomDetails);
    }
    closeEditRoomPopup = () => {
        this.setState({openEditRoomPopup:false});
    }

    handleDeleteClickMenuItem = () => {
        this.setState({openMoreDetails: !this.state.openMoreDetails});
        this.props.deleteRoom(this.state.roomDetails["id"],this.state.roomDetails["identity"]);
    }

    displayRoom = (userIdentity,id) => {
        // console.log(this.props.history);
        // this.props.history.push({
        //     pathname: `${encodeURIComponent(userIdentity)}/${id}`
        // })
        // console.log(this.props.history);
        this.props.clickRoom(userIdentity,id)
    }

    

    render() {
        console.log(this.props.selectedUserName.SelectedUserName);
        return (
            <div>
                <Box sx={{ width: '100%' }}>
                    <TableContainer>
                        <Table
                            sx={{ minWidth: 750 }}
                            aria-labelledby="tableTitle"
                            size={'medium'}
                        >
                        <TableHead sx={{ fontWeight: '700', textTransform:'capitalize'}}>
                            <TableRow>
                                {this.props.columns.map((ColumnName)=>{
                                    return (
                                        <TableCell align="center">
                                            <TableSortLabel
                                                active={this.state.sortedColumn === ColumnName ? true:false}
                                                direction={this.state.sortType}
                                                onClick={()=>{this.handleSortColumn(ColumnName)}}
                                            >
                                                {ColumnName}
                                                </TableSortLabel>
                                            
                                        </TableCell>
                                    )
                                })}
                            </TableRow>
                        </TableHead>
                        <TableBody>
                            {this.visibleRows().length === 0 ? 
                            (
                                <TableRow>
                                    <TableCell colSpan={this.props.columns.length} style={{color:'rgb(144 140 140 / 87%)',fontWeight:'500',fontSize:'1.25rem',textAlign:'center'
                                    
                                    }}>
                                        No Listing Found
                                    </TableCell>
                                </TableRow>
                            ) : 
                            (<React.Fragment>
                                {this.visibleRows(this.state.sortedColumn,this.state.sortType).map((rowData)=>{
                                console.log(rowData);
                                return(
                                    <TableRow>
                                        {this.props.columns.map((individualData)=>{
                                            if(individualData==="more") {
                                                return (
                                                    <>
                                                    <TableCell onClick={(e)=>this.handleMoreDetails(e.currentTarget,rowData)} style={{padding:'12px'}}>{rowData[individualData]}</TableCell>
                                                    {this.state.openMoreDetails &&  (
                                                        // <DCCRowv1 open={this.state.openMoreDetails}></DCCRowv1>
                                                        <Menu
                                                            id="basic-menu"
                                                            open={this.state.openMoreDetails}
                                                            onClose={this.handleCloseMenu}
                                                            anchorEl={this.state.anchorElElement}
                                                            anchorOrigin={{
                                                                vertical: 'top',
                                                                horizontal: 'right',
                                                              }}
                                                            MenuListProps={{
                                                            'aria-labelledby': 'basic-button',
                                                            }}
                                                        >
                                                            <MenuItem onClick={()=>this.handleEditClickMenuItem()}>Edit</MenuItem>
                                                            <MenuItem onClick={()=>this.handleDeleteClickMenuItem()}>Delete</MenuItem>
                                                        </Menu>
                                                    )}
                                                    </>
                                                )
                                            }
                                            else {
                                                return <TableCell onClick={individualData === "name" ? ()=>this.displayRoom(rowData["identity"],rowData["id"]) : ()=>{}}>{rowData[individualData]}</TableCell>
                                            }
                                            
                                        })}
                                        
                                    </TableRow>
                                )
                            })}
                            </React.Fragment>
                        )}
                        </TableBody>
                        </Table>
                    </TableContainer>
                </Box>
            </div>
        )
    }
}

const mapStateToProps = (state=rootReducer) => ({
    selectedUserName: state.SelectedUserName,
})

export default connect(mapStateToProps)(withRouter(DCCGridv2));