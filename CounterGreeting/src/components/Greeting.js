import React from "react";

// CLASS COMPONENT: state is managed with this.state / this.setState
class Greeting extends React.Component {
  constructor(props) {
    super(props);

    this.state = {
      name: "",
      greeting: "",
      error: "",
    };

    // Bind class event handlers so `this` points to the component
    this.handleNameChange = this.handleNameChange.bind(this);
    this.handleGreet = this.handleGreet.bind(this);
  }

  handleNameChange(event) {
    this.setState({ name: event.target.value });
  }

  handleGreet() {
    const trimmedName = this.state.name.trim();

    if (trimmedName === "") {
      this.setState({ error: "Please enter your name" });
    } else {
      this.setState({ greeting: trimmedName, error: "" });
    }
  }

  render() {
    const { name, greeting, error } = this.state;

    return (
      <section className="section">
        <p id="classLabel" className="badge badge-class">Class Component (this.state)</p>

        <input
          id="nameInput"
          className={error ? "text-input input-error" : "text-input"}
          type="text"
          placeholder="Enter your name"
          value={name}
          onChange={this.handleNameChange}
        />
        <button id="greetBtn" className="save-btn" onClick={this.handleGreet}>
          Greet
        </button>

        {error && <p id="nameError" className="message message-error">{error}</p>}
        {greeting && (
          <p id="greeting" className="message message-ok">{`Hello, ${greeting}!`}</p>
        )}
      </section>
    );
  }
}

export default Greeting;