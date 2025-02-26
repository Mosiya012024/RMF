// import { Description } from "@mui/icons-material";

import { useCallback, useState } from "react";
import { useMemo } from "react";
import React from "react";
import Prep2 from "./Prep2";

// class InterviewPrep extends React.Component {
//     constructor() {
//         super(props);
//         this.state = {

//         }
//     }
    
//     handleJoin =() => {
//         const obj = new Object();
//         obj.name = "Mosiya";
//         obj.Dscription = "Noo one can";


//         const abcd = {
//             Name : "Mosiya",
//             Description : "jwdhjd wrfehrf"
//         }

//         const abcde = Object.create(abcd);
//     }

//     render() {
//         return (
//             <></>
//         )
//     }
// }

// export default InterviewPrep();

function InterviewPrep() {
    const [age,setAge] = useState(10);
    const [salary,setSalary] = useState(100);
    const [count,setCount] = useState(0);
    const [counter1,setCounter1] = useState(0);
    const [counter2,setCounter2] = useState(0);

    const incrementAge = useCallback(() => {
        setAge(age+1);
        console.log("increment age called")
    },[age]);

    const incrementSalary = useCallback(() => {
        setSalary(salary+10);
        console.log("increment salary called")
    },[salary]);

  //   const incrementAge = () => {
  //     setAge(age+1);
  //     console.log("increment age called")
  //   }

  // const incrementSalary = () => {
  //     setSalary(salary+10);
  //     console.log("increment salary called")
  // };
  //9765711125
    //useCallback hook is used in situations where you are passing the functions to child components. and that child component invokes the callback functions. then if the same child component is rendered more than once in parent com
    //then how many child comps are there, then that many times the call back functions willbe called. So to avoid this, we will be using useCallback hook along with dependency array
    const isEven = useMemo(() => {
        let i=1;
        while(i<=2000000000) i++;
        console.log("rfrfffffffffffff")
        return counter1%2 === 0;
    },[counter1]);

    const x1 = 100;
    function  A() {
        const x1 = 200;
        console.log(x1); // Output?
    }
    console.log(x1);
    A();

    for (var i = 0; i < 3; i++) {
        setTimeout(function () {
          console.log(i); // Output?
        }, 1000);
      }
      
      for (let j = 0; j < 3; j++) {
        setTimeout(function () {
          console.log(j); // Output?
        }, 1000);
      }

      for (var i = 10; i < 13; i++) {
        setTimeout(function() {
          console.log(i);
        },  2000);
      }
      
      for (let j = 20; j < 23; j++) {
        setTimeout(function() {
          console.log(j);
        }, 2000);
      }
    
      console.log("It is again based on performance");
    return(
        <div style={{ position:'absolute', top:'100px'}}>
            <div>Mosiya</div>
            <div>age is: {age}</div>
            <Prep2 handleClick={incrementAge}>Change Age</Prep2>
            <div>salary is: {salary}</div>
            <Prep2 handleClick={incrementSalary}>Change Salary</Prep2>
            <br></br><br></br>
            <button onClick={()=>setCounter1(counter1+1)}>counter1 - {counter1}</button>
            <div>{isEven ? 'even' : 'odd'}</div>
            <div>{isEven}</div>
            <button onClick={()=>setCounter2(counter2+1)}>counter2 - {counter2}</button>
        </div>
    )
}

const data = {
  name: "Mosiya",
  age: 24,
  number : 577,
  greet: function() {
    console.log("greeting here");
  }
}

const dataObject = Object.create(data);
dataObject.greet();

// let a  = 10;
// if(a) {
//   var a = 20;
// }
//Error like Cannot redeclare block-scoped variable 'a'
export default InterviewPrep;