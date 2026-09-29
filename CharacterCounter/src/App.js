import React, { useState } from "react";

const MAX_LENGTH = 100;

function App() {
  // The text area is a controlled input: its value always comes from state
  const [message, setMessage] = useState("");

  // Input/change event: read the new text from event.target.value
  const handleChange = (event) => {
    setMessage(event.target.value);
  };

  const handleClear = () => {
    setMessage("");
  };

  const length = message.length;
  const isExceeded = length > MAX_LENGTH;
  const progress = Math.min((length / MAX_LENGTH) * 100, 100);

  return (
    <div className="page">
      <div className="counter-card">
        <h1 id="title" className="counter-title">Character Counter</h1>

        <textarea
          id="messageInput"
          className={isExceeded ? "message-input input-error" : "message-input"}
          placeholder="Type your message here..."
          rows="5"
          value={message}
          onChange={handleChange}
        />

        <div className="progress-track">
          <div
            className={isExceeded ? "progress-bar progress-over" : "progress-bar"}
            style={{ width: `${progress}%` }}
          />
        </div>

        <p id="charCount" className={isExceeded ? "char-count count-over" : "char-count"}>
          {`${length} / ${MAX_LENGTH}`}
        </p>

        {isExceeded && (
          <p id="warning" className="warning">Character limit exceeded!</p>
        )}

        <div className="preview-box">
          <span className="preview-heading">Live Preview</span>
          <p id="preview" className="preview-text">{message}</p>
        </div>

        <button id="clearBtn" className="clear-btn" onClick={handleClear}>
          Clear
        </button>
      </div>
    </div>
  );
}

export default App;