import { Component } from "react";

class InterviewPrep4 extends Component {
    constructor(props) {
        super(props);
        this.state = {

        }
    }

    componentDidMount() {
        this.reduceFunc();
    }

    reduceFunc() {
        let arr = [10,20,30,40,50,60,70,80];

        let sum = arr.reduce((acc,curr)=>{acc = acc+curr;
            return acc;
        },0);
        console.log(sum);
        //way 1
        let initialValue = 0;
        let ans = arr.reduce((accumulator,curr_value)=>accumulator+curr_value, initialValue);
        console.log(ans);
        //way 2
        let ans2 = arr.reduce((a,b)=>a+b);
        console.log(ans2+90);
        //way3
        const total = arr.reduce((accumulator,curr_value,index) => {
            console.log(index);
            return accumulator+curr_value;
        },0);
        console.log(total);
        //way 4 by using a function inside reduce function btw in the above also, we are using a arrow callback function
        //Here we are using a normal callback function.
        const total2 = arr.reduce(this.reduceCallBack,15)
        console.log(total2);

        //how it is used in React if the data is stored in the form of JSON Objects.
        const shoppingCart = [
            {
              id: 1,
              product: 'Fjallraven - Foldsack No. 1 Backpack, Fits 15 Laptops',
              qty: 1,
              price: 109.95,
            },
            {
              id: 2,
              product: 'Mens Casual Premium Slim Fit T-Shirts ',
              qty: 1,
              price: 22.3,
            },
            {
              id: 3,
              product: 'Mens Cotton Jacket',
              qty: 2,
              price: 55.99,
            },
            {
              id: 4,
              product: 'Mens Casual Slim Fit',
              qty: 1,
              price: 15.99,
            },
        ];
        //applyting custom logic as well
        let shoppingKartAns = shoppingCart.reduce((accumulator,curr_value)=> {
            return accumulator+curr_value.qty*curr_value.price;
        },0);
        console.log(shoppingKartAns);

        //another way for above.
        let shoppingKartAns2 = shoppingCart.reduce((accumulator,curr_value) => {
            return accumulator+curr_value.qty*curr_value.price;
        },shoppingCart[0].qty*shoppingCart[0].price);
        console.log(shoppingKartAns2);

        //another way for above.
        let shoppingKartAns3 = shoppingCart.reduce((accumulator,curr_value) => {
            return accumulator+curr_value.qty*curr_value.price;
        });
        console.log(shoppingKartAns3);// here you get the output as spmething like Object object.
        //this is because as we are not passing the intial_value here , it takes the array[0] which is an object
        // then object+second_value is some number, so object+number is not equal to number right? so 

        //another way of doing this.
        let shoppingKartAns4 = shoppingCart.reduce(this.shoppingKartCallBackFunc,0);
        console.log(shoppingKartAns4);

    }

    shoppingKartCallBackFunc = (accumulator,curr_value) => {
        return accumulator+curr_value.qty*curr_value.price;
    }


















    
    reduceCallBack = (accumulator, curr_value) => {
        return accumulator+curr_value;
    }

    render() {
        return (
            <div>

            </div>
        )
    }
}

export default InterviewPrep4;