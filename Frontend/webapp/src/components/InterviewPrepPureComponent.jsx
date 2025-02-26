import { PureComponent } from "react";
import { Component } from "react";

class InterviewPrepPureComponent extends PureComponent {
    constructor(props) {
        super(props);
        this.state = {
            name: "Laiba naraaz hojaati hai yaar"
        }
    }


    componentDidUpdate() {
        setInterval(()=>{
            console.log("Laiba ka set Interval start hogaya")
            this.setState({name:"Laiba phir se naraaz hojaati hai yaar"});
        },[5000]);
    }


    handleNameChange = () => {
        this.setState({name: "Ewww"});
    }

    render() {
        let num = "Rohit Saraf";
        num = "Prajakta Koli";
        console.log(num);
        console.log("Laiba render mein aagayi")

        let numbers = [1,3,5,7,9];
        if (Array.prototype.map) {
            Array.prototype.map = function(callback, thisArg) {
              const result = [];
              for (let i = 0; i < this.length; i++) {
                if (this.hasOwnProperty(i)) {
                  result.push(callback.call(thisArg, this[i], i, this));
                }
              }
              return result;
            };
        }
          
        let result = numbers.map((x)=>{
            return x*x;
        })
        console.log(result);
        return(
            <>
            <div onClick={()=>this.handleNameChange()}>{this.state.name}</div>
            </>
        )
    }
}

export default InterviewPrepPureComponent;