import React from "react";
class TestFile extends React.Component {

    constructor(props) {
        super(props);
        this.state = {
          isOpen: false,
          selectedOption: null,
        };
      }
    
      toggleDropdown = () => {
        this.setState((prevState) => ({
          isOpen: !prevState.isOpen,
        }));
      };
    
      handleOptionClick = (option) => {
        this.setState({
          selectedOption: option,
          isOpen: false,
        });
      };
    
      render() {
        const { isOpen, selectedOption } = this.state;
        const options = ['Option 1', 'Option 2', 'Option 3'];
    
        return (
          <div className="dropdown">
            <button onClick={this.toggleDropdown} className="dropdown-button">
              {selectedOption || 'Select an option'}
            </button>
            {isOpen && (
              <div className="dropdown-menu">
                {options.map((option, index) => (
                  <div
                    key={index}
                    onClick={() => this.handleOptionClick(option)}
                    className="dropdown-item"
                  >
                    {option}
                  </div>
                ))}
              </div>
            )}
          </div>
        );
      }
}
export default TestFile;