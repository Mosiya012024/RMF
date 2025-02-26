import { TableContainer, TableHead, TableRow,TableCell, Table, TableBody, TableSortLabel } from "@mui/material";
import Box from '@mui/material/Box';
import { Component } from "react";



class DCCGridv1 extends Component {
    constructor(props) {
        super(props);
        
        this.state = {
            sortType: 'asc',
            sortedColumn: this.props.columns[0],
            sortedData: this.sortData(this.props.columns[0],'asc',""),
        }
        this.handleSortColumn = this.handleSortColumn.bind(this);
        // this.sortData = this.sortData.bind(this);
        // this.sortData(this.state.sortedColumn,this.state.sortType);
        //this.props.rowsData.sort((a, b) => a[this.state.sortedColumn].localeCompare(b[this.state.sortedColumn]));
    }

    componentDidMount = () => {
        this.sortData(this.state.sortedColumn,this.state.sortType)
    }

    sortData = (column, type,setType) => {
        console.log("Original rowsData:", this.props.rowsData);
    
        const sortedData = [...this.props.rowsData]; // Create a shallow copy
        console.log("Shallow copy before sorting:", sortedData);
    
        sortedData.sort((a, b) => {
            if(column === "amount" || column === "vacancy") {
                if (type === 'asc') {
                    return a[column]-b[column];
                } else {
                    return b[column]-a[column];
                } 
            }
            else {
                if (type === 'asc') {
                    return a[column].localeCompare(b[column]);
                } else {
                    return b[column].localeCompare(a[column]);
                }
            }
            
        });
    
        console.log("Sorted Data:", sortedData);
        if(setType === "") {
            return sortedData;
        }
        else {
            this.setState({ sortedData }, () => {
                console.log("State updated with sortedData:", this.state.sortedData);
            });
        }
    }
    
    sortDatas = (column,type) => {
        console.log(this.props.rowsData)
        const sortedData = [...this.props.rowsData];
        if(type === 'desc') {
            sortedData.sort((a, b) => {
                if (a[column] > b[column]) return -1;
                if (a[column] < a[column]) return 1;
                return 0;
            });
        }
        else {
            sortedData.sort((a, b) => a[column].localeCompare(b[column]));
        }
        console.log(sortedData);
        this.setState({ sortedData }, () => {
            console.log("State updated with sortedData:", this.state.sortedData);
        });
    }

    // componentDidUpdate = (prevProps,prevState) => {
    //     // if(this.state.sortType === 'desc') {
    //     //     this.props.rowsData.sort((a, b) => {
    //     //         if (a[this.state.sortedColumn] > b[this.state.sortedColumn]) return -1;
    //     //         if (a[this.state.sortedColumn] < a[this.state.sortedColumn]) return 1;
    //     //         return 0;
    //     //     });
    //     //     return;
    //     // }
    //     // this.props.rowsData.sort((a, b) => a[this.state.sortedColumn].localeCompare(b[this.state.sortedColumn]));
    //     if (prevState.sortType !== this.state.sortType || prevState.sortedColumn !== this.state.sortedColumn) {
    //         this.sortData(this.state.sortedColumn, this.state.sortType);
    //     }
    // }

    handleSortColumn = (ColumnName) => {
        if(this.state.sortedColumn === ColumnName) {
            this.setState({sortType: this.state.sortType === 'asc' ? 'desc' : 'asc'},
                // ()=>{this.sortData(this.state.sortedColumn,this.state.sortType)}
            );
            return;
        }
        this.setState({sortedColumn: ColumnName,sortType: 'asc'},
            // ()=>{this.sortData(this.state.sortedColumn,this.state.sortType)}
        );
    }

    visibleRows = (column,type) => {
        return this.sortData(column,type,"");
    }

    render() {
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
                            {/* {this.state.sortedData.map((rowData)=>{
                                return(
                                    <TableRow>
                                        {this.props.columns.map((individualData)=>{
                                            return <TableCell>{rowData[individualData]}</TableCell>
                                        })}
                                    </TableRow>
                                )
                            })} */}
                            {this.visibleRows(this.state.sortedColumn,this.state.sortType).map((rowData)=>{
                                return(
                                    <TableRow>
                                        {this.props.columns.map((individualData)=>{
                                            return <TableCell>{rowData[individualData]}</TableCell>
                                        })}
                                    </TableRow>
                                )
                            })}
                        </TableBody>
                        </Table>
                    </TableContainer>
                </Box>
            </div>
        )
    }
}
export default DCCGridv1