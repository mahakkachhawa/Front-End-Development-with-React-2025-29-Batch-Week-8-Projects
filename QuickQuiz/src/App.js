import React, { useState } from "react";

// Requirement 1: this array is provided in the starter code (do not change it).
// `correctIndex` is the position of the right answer inside `options`.
const questions = [
  {
    question: "Which hook is used to add state to a functional component?",
    options: ["useProps", "useState", "useClick", "useRender"],
    correctIndex: 1,
  },
  {
    question: "Which method is used to update state in a class component?",
    options: ["this.updateState()", "this.changeState()", "this.setState()", "this.newState()"],
    correctIndex: 2,
  },
  {
    question: "What does event.target.value return in an input change event?",
    options: [
      "The current text of the input",
      "The name of the component",
      "The id of the input",
      "The previous state",
    ],
    correctIndex: 0,
  },
];

function App() {
  const [currentIndex, setCurrentIndex] = useState(0);
  const [selectedOption, setSelectedOption] = useState(null); // null = not answered yet
  const [score, setScore] = useState(0);
  const [showResult, setShowResult] = useState(false);

  const currentQuestion = questions[currentIndex];
  const isAnswered = selectedOption !== null;
  const isLastQuestion = currentIndex === questions.length - 1;

  const handleOptionClick = (index) => {
    if (isAnswered) return; // a question can only be answered once
    setSelectedOption(index);
    if (index === currentQuestion.correctIndex) {
      setScore((prev) => prev + 1); // functional update
    }
  };

  const handleNext = () => {
    if (isLastQuestion) {
      setShowResult(true);
    } else {
      setCurrentIndex((prev) => prev + 1);
      setSelectedOption(null); // clears feedback and re-enables the options
    }
  };

  const handleRestart = () => {
    setCurrentIndex(0);
    setSelectedOption(null);
    setScore(0);
    setShowResult(false);
  };

  // Feedback text is derived from state (nothing extra to store)
  let feedbackText = "";
  if (isAnswered) {
    feedbackText =
      selectedOption === currentQuestion.correctIndex
        ? "Correct!"
        : `Wrong! Correct answer: ${currentQuestion.options[currentQuestion.correctIndex]}`;
  }

  const optionClass = (index) => {
    if (!isAnswered) return "option-btn";
    if (index === currentQuestion.correctIndex) return "option-btn option-correct";
    if (index === selectedOption) return "option-btn option-wrong";
    return "option-btn";
  };

  return (
    <div className="page">
      <div className="quiz-card">
        <h1 id="title" className="quiz-title">Quick Quiz</h1>

        {!showResult ? (
          <div className="question-area">
            <div className="quiz-meta">
              <p id="questionNumber" className="question-number">
                {`Question ${currentIndex + 1} of ${questions.length}`}
              </p>
              <p id="score" className="score">{`Score: ${score}`}</p>
            </div>

            <h2 id="questionText" className="question-text">{currentQuestion.question}</h2>

            <div className="options">
              {currentQuestion.options.map((option, index) => (
                <button
                  key={index}
                  id={`option${index}`}
                  className={optionClass(index)}
                  disabled={isAnswered}
                  onClick={() => handleOptionClick(index)}
                >
                  {option}
                </button>
              ))}
            </div>

            {isAnswered && (
              <>
                <p
                  id="feedback"
                  className={
                    selectedOption === currentQuestion.correctIndex
                      ? "feedback feedback-correct"
                      : "feedback feedback-wrong"
                  }
                >
                  {feedbackText}
                </p>
                <button id="nextBtn" className="action-btn" onClick={handleNext}>
                  {isLastQuestion ? "See Result" : "Next"}
                </button>
              </>
            )}
          </div>
        ) : (
          <div className="result-view">
            <div id="result" className="result">
              {`You scored ${score} out of ${questions.length}`}
            </div>
            <button id="restartBtn" className="action-btn" onClick={handleRestart}>
              Restart
            </button>
          </div>
        )}
      </div>
    </div>
  );
}

export default App;