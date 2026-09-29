import React, { useState } from "react";

const MIN_QUANTITY = 1;
const MAX_QUANTITY = 10;

function App() {
  const [quantity, setQuantity] = useState(MIN_QUANTITY);

  // Adds 1, but never goes above the maximum
  const handleIncrease = () => {
    setQuantity((prev) => Math.min(prev + 1, MAX_QUANTITY));
  };

  // Removes 1, but never goes below the minimum
  const handleDecrease = () => {
    setQuantity((prev) => Math.max(prev - 1, MIN_QUANTITY));
  };

  // Calls the setter three times in ONE handler.
  // The functional form (prev => ...) is required here: each call receives
  // the latest value, so the three updates add up instead of overwriting.
  const handleAddThree = () => {
    setQuantity((prev) => Math.min(prev + 1, MAX_QUANTITY));
    setQuantity((prev) => Math.min(prev + 1, MAX_QUANTITY));
    setQuantity((prev) => Math.min(prev + 1, MAX_QUANTITY));
  };

  // Conditional message based on state
  let limitMessage = null;
  if (quantity === MAX_QUANTITY) {
    limitMessage = "Maximum limit reached";
  } else if (quantity === MIN_QUANTITY) {
    limitMessage = "Minimum quantity is 1";
  }

  return (
    <div className="page">
      <div className="selector-card">
        <h1 id="title" className="selector-title">Quantity Selector</h1>

        <div className="stepper">
          <button id="decreaseBtn" className="step-btn" onClick={handleDecrease}>
            -
          </button>
          <span id="quantity" className="quantity">{quantity}</span>
          <button id="increaseBtn" className="step-btn" onClick={handleIncrease}>
            +
          </button>
        </div>

        <button id="addThreeBtn" className="bulk-btn" onClick={handleAddThree}>
          +3
        </button>

        {limitMessage && (
          <p
            id="limitMessage"
            className={
              quantity === MAX_QUANTITY ? "limit-message limit-max" : "limit-message limit-min"
            }
          >
            {limitMessage}
          </p>
        )}
      </div>
    </div>
  );
}

export default App;