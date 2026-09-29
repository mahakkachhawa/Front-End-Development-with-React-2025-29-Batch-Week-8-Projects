import React, { useState } from "react";

function App() {
  // Requirement 7: the ticket count is stored with the useState hook
  const [tickets, setTickets] = useState(0);

  const handleAddTicket = () => {
    setTickets((prev) => prev + 1);
  };

  const handleReset = () => {
    setTickets(0);
  };

  return (
    <div className="page">
      <div className="ticket-card">
        <h1 id="title" className="ticket-title">Ticket Counter</h1>

        <div className="count-row">
          <p id="ticketLabel" className="ticket-label">Tickets booked:</p>
          <span id="ticketCount" className="ticket-count">{tickets}</span>
        </div>

        <div className="button-row">
          <button id="addTicket" className="btn btn-primary" onClick={handleAddTicket}>
            Add Ticket
          </button>
          <button id="resetTickets" className="btn btn-secondary" onClick={handleReset}>
            Reset
          </button>
        </div>
      </div>
    </div>
  );
}

export default App;