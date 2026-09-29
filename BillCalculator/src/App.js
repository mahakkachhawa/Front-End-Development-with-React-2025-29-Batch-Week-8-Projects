import React, { useState } from "react";

const TIP_OPTIONS = [
  { id: "tip10Btn", percent: 10 },
  { id: "tip15Btn", percent: 15 },
  { id: "tip20Btn", percent: 20 },
];

function App() {
  // Requirement 13: three separate state variables
  const [bill, setBill] = useState(""); // kept as a string so the input can be empty
  const [tipPercent, setTipPercent] = useState(0);
  const [people, setPeople] = useState(1);

  // Input/change event: read the typed value from event.target.value
  const handleBillChange = (event) => {
    setBill(event.target.value);
  };

  const handleIncreasePeople = () => {
    setPeople((prev) => prev + 1);
  };

  // Functional update with a lower limit of 1 person
  const handleDecreasePeople = () => {
    setPeople((prev) => Math.max(prev - 1, 1));
  };

  const handleReset = () => {
    setBill("");
    setTipPercent(0);
    setPeople(1);
  };

  // Derived values: calculated from state on every render (no extra state needed)
  const parsedBill = parseFloat(bill);
  const billAmount = isNaN(parsedBill) || parsedBill < 0 ? 0 : parsedBill; // empty -> 0
  const tipAmount = (billAmount * tipPercent) / 100;
  const totalAmount = billAmount + tipAmount;
  const perPerson = totalAmount / people;

  return (
    <div className="page">
      <div className="bill-card">
        <h1 id="title" className="bill-title">Bill Calculator</h1>

        <label className="field-label" htmlFor="billInput">Bill amount</label>
        <div className="bill-field">
          <span className="currency">₹</span>
          <input
            id="billInput"
            className="bill-input"
            type="number"
            min="0"
            placeholder="0.00"
            value={bill}
            onChange={handleBillChange}
          />
        </div>

        <div className="section-row">
          <span className="field-label">Select tip</span>
          <span className="tip-selected">
            <span id="tipPercent">{tipPercent}</span>%
          </span>
        </div>
        <div className="tip-row">
          {TIP_OPTIONS.map((option) => (
            <button
              key={option.id}
              id={option.id}
              className={tipPercent === option.percent ? "tip-btn tip-btn-active" : "tip-btn"}
              onClick={() => setTipPercent(option.percent)}
            >
              {option.percent}%
            </button>
          ))}
        </div>

        <div className="section-row">
          <span className="field-label">Number of people</span>
        </div>
        <div className="people-row">
          <button id="decreasePeople" className="round-btn" onClick={handleDecreasePeople}>
            -
          </button>
          <span id="peopleCount" className="people-count">{people}</span>
          <button id="increasePeople" className="round-btn" onClick={handleIncreasePeople}>
            +
          </button>
        </div>

        <div className="results">
          <div className="result-row">
            <span className="result-label">Tip amount</span>
            <span className="result-value">₹ <span id="tipAmount">{tipAmount.toFixed(2)}</span></span>
          </div>
          <div className="result-row">
            <span className="result-label">Total amount</span>
            <span className="result-value">₹ <span id="totalAmount">{totalAmount.toFixed(2)}</span></span>
          </div>
          <div className="result-row result-highlight">
            <span className="result-label">Per person</span>
            <span className="result-value">₹ <span id="perPerson">{perPerson.toFixed(2)}</span></span>
          </div>
        </div>

        <button id="resetBtn" className="reset-btn" onClick={handleReset}>
          Reset
        </button>
      </div>
    </div>
  );
}

export default App;