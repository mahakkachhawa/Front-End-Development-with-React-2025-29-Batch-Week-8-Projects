import React, { useState } from "react";

const RATINGS = [1, 2, 3, 4, 5];

// Returns the message for a rating (or null when nothing is selected yet)
function getRatingMessage(rating) {
  if (rating === 0) return null;
  if (rating <= 2) return "We will try to do better";
  if (rating === 3) return "Thanks for the feedback";
  return "Glad you liked it!";
}

function App() {
  // Requirement 12: three separate state variables
  const [rating, setRating] = useState(0); // 0 means "no rating selected"
  const [comment, setComment] = useState("");
  const [submitted, setSubmitted] = useState(false);

  const handleCommentChange = (event) => {
    setComment(event.target.value);
  };

  const handleSubmit = () => {
    if (rating === 0) return; // safety guard: a rating is required
    setSubmitted(true);
  };

  const handleNewFeedback = () => {
    setRating(0);
    setComment("");
    setSubmitted(false);
  };

  const ratingMessage = getRatingMessage(rating);
  const trimmedComment = comment.trim();

  return (
    <div className="page">
      <div className="feedback-card">
        <h1 id="title" className="feedback-title">Feedback Widget</h1>

        {!submitted ? (
          <>
            <div className="rating-row">
              {RATINGS.map((value) => (
                <button
                  key={value}
                  id={`star${value}`}
                  className={rating === value ? "rating-btn rating-btn-active" : "rating-btn"}
                  aria-pressed={rating === value}
                  onClick={() => setRating(value)}
                >
                  {value}
                </button>
              ))}
            </div>

            <p id="selectedRating" className="selected-rating">
              {rating === 0 ? "No rating selected" : `Selected rating: ${rating}`}
            </p>

            {ratingMessage && (
              <p id="ratingMessage" className="rating-message">{ratingMessage}</p>
            )}

            <textarea
              id="feedbackInput"
              className="feedback-input"
              placeholder="Tell us more (optional)..."
              rows="4"
              value={comment}
              onChange={handleCommentChange}
            />

            <button
              id="submitBtn"
              className="submit-btn"
              disabled={rating === 0}
              onClick={handleSubmit}
            >
              Submit
            </button>
          </>
        ) : (
          <div className="thanks-view">
            <div id="thankYou" className="thank-you">
              {`Thank you for your ${rating}-star feedback!`}
            </div>

            {trimmedComment !== "" && (
              <p id="submittedComment" className="submitted-comment">
                {`Your comment: ${trimmedComment}`}
              </p>
            )}

            <button id="newFeedbackBtn" className="submit-btn" onClick={handleNewFeedback}>
              Give New Feedback
            </button>
          </div>
        )}
      </div>
    </div>
  );
}

export default App;