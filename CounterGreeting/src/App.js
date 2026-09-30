import React from "react";
import Counter from "./components/Counter";
import Greeting from "./components/Greeting";

function App() {
  return (
    <div className="page">
      <div className="card">
        <h1 id="title" className="card-title">Counter and Greeting</h1>
        <Counter />
        <Greeting />
      </div>
    </div>
  );
}

export default App;