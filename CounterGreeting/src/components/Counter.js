import React, { useState } from "react";

const MIN_COUNT = 0;
const HIGH_COUNT = 5;

// FUNCTIONAL COMPONENT: state is managed with the useState hook
function Counter() {
  const [count, setCount] = useState(MIN_COUNT);

  const handleIncrease = () => {
    setCount((prev) => prev + 1);
  };

  const handleDecrease = () => {
    setCount((prev) => Math.max(prev - 1, MIN_COUNT));
  };

  const handleReset = () => {
    setCount(MIN_COUNT);
  };

  return (
    <section className="section">
      <p id="functionalLabel" className="badge badge-functional">
        Functional Component (useState)
      </p>

      <div className="stepper">
        <button id="decreaseBtn" className="step-btn" onClick={handleDecrease}>-</button>
        <span id="count" className="count">{count}</span>
        <button id="increaseBtn" className="step-btn" onClick={handleIncrease}>+</button>
      </div>

      <button id="resetBtn" className="link-btn" onClick={handleReset}>Reset</button>

      {count >= HIGH_COUNT && (
        <p id="countMessage" className="message message-ok">High count!</p>
      )}
    </section>
  );
}

export default Counter;