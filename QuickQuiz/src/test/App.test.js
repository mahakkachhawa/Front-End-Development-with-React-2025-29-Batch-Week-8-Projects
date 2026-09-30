import React from "react";
import { render, fireEvent } from "@testing-library/react";
import "@testing-library/jest-dom";
import App from "../App";

// Starter data (correct option index): Q1 -> 1, Q2 -> 2, Q3 -> 0
const Q1_TEXT = "Which hook is used to add state to a functional component?";
const Q2_TEXT = "Which method is used to update state in a class component?";
const Q3_TEXT = "What does event.target.value return in an input change event?";

const text = (container, selector) => container.querySelector(selector).textContent;

const chooseOption = (container, index) => {
  fireEvent.click(container.querySelector(`#option${index}`));
};

const answerAndContinue = (container, index) => {
  chooseOption(container, index);
  fireEvent.click(container.querySelector("#nextBtn"));
};

describe("Quick Quiz App", () => {
  test("verify_heading_rendering", () => {
    const { container } = render(<App />);
    const title = container.querySelector("h1#title");
    expect(title).toBeInTheDocument();
    expect(title).toHaveTextContent("Quick Quiz");
  });

  test("verify_first_question_screen_rendering", () => {
    const { container } = render(<App />);
    expect(container.querySelector("p#questionNumber").textContent).toBe("Question 1 of 3");
    expect(container.querySelector("h2#questionText").textContent).toBe(Q1_TEXT);
    expect(container.querySelector("p#score").textContent).toBe("Score: 0");

    const expectedOptions = ["useProps", "useState", "useClick", "useRender"];
    expectedOptions.forEach((label, i) => {
      const btn = container.querySelector(`button#option${i}`);
      expect(btn).toBeInTheDocument();
      expect(btn.textContent.trim()).toBe(label);
      expect(btn).toBeEnabled();
    });
  });

  test("verify_feedback_and_next_button_hidden_before_answering", () => {
    const { container } = render(<App />);
    expect(container.querySelector("#feedback")).not.toBeInTheDocument();
    expect(container.querySelector("#nextBtn")).not.toBeInTheDocument();
    expect(container.querySelector("#result")).not.toBeInTheDocument();
  });

  test("verify_correct_answer_shows_feedback_and_increases_score", () => {
    const { container } = render(<App />);
    chooseOption(container, 1); // useState
    expect(container.querySelector("p#feedback").textContent).toBe("Correct!");
    expect(text(container, "#score")).toBe("Score: 1");
    expect(container.querySelector("button#nextBtn")).toBeInTheDocument();
  });

  test("verify_wrong_answer_shows_correct_answer_and_keeps_score", () => {
    const { container } = render(<App />);
    chooseOption(container, 0); // useProps
    expect(text(container, "#feedback")).toBe("Wrong! Correct answer: useState");
    expect(text(container, "#score")).toBe("Score: 0");
  });

  test("verify_options_are_disabled_after_answering", () => {
    const { container } = render(<App />);
    chooseOption(container, 0); // wrong answer first
    for (let i = 0; i < 4; i++) {
      expect(container.querySelector(`#option${i}`)).toBeDisabled();
    }

    // Trying to answer again must not change the feedback or the score
    chooseOption(container, 1);
    expect(text(container, "#feedback")).toBe("Wrong! Correct answer: useState");
    expect(text(container, "#score")).toBe("Score: 0");
  });

  test("verify_next_button_moves_to_next_question_and_resets_feedback", () => {
    const { container } = render(<App />);
    chooseOption(container, 1);
    const nextBtn = container.querySelector("#nextBtn");
    expect(nextBtn.textContent.trim()).toBe("Next");
    fireEvent.click(nextBtn);

    expect(text(container, "#questionNumber")).toBe("Question 2 of 3");
    expect(text(container, "#questionText")).toBe(Q2_TEXT);
    expect(text(container, "#score")).toBe("Score: 1");
    expect(container.querySelector("#feedback")).not.toBeInTheDocument();
    expect(container.querySelector("#nextBtn")).not.toBeInTheDocument();
    for (let i = 0; i < 4; i++) {
      expect(container.querySelector(`#option${i}`)).toBeEnabled();
    }
  });

  test("verify_last_question_button_says_see_result", () => {
    const { container } = render(<App />);
    answerAndContinue(container, 1); // Q1
    answerAndContinue(container, 2); // Q2
    expect(text(container, "#questionNumber")).toBe("Question 3 of 3");
    expect(text(container, "#questionText")).toBe(Q3_TEXT);

    chooseOption(container, 0); // Q3
    expect(container.querySelector("#nextBtn").textContent.trim()).toBe("See Result");
  });

  test("verify_result_screen_after_all_correct_answers", () => {
    const { container } = render(<App />);
    answerAndContinue(container, 1);
    answerAndContinue(container, 2);
    answerAndContinue(container, 0); // "See Result"

    expect(container.querySelector("div#result").textContent).toBe("You scored 3 out of 3");
    expect(container.querySelector("#questionNumber")).not.toBeInTheDocument();
    expect(container.querySelector("#questionText")).not.toBeInTheDocument();
    expect(container.querySelector("#option0")).not.toBeInTheDocument();
    expect(container.querySelector("#feedback")).not.toBeInTheDocument();
    expect(container.querySelector("#nextBtn")).not.toBeInTheDocument();
  });

  test("verify_restart_returns_to_first_question_with_zero_score", () => {
    const { container } = render(<App />);
    answerAndContinue(container, 1); // correct
    answerAndContinue(container, 0); // wrong
    answerAndContinue(container, 3); // wrong
    expect(text(container, "#result")).toBe("You scored 1 out of 3");

    const restartBtn = container.querySelector("button#restartBtn");
    expect(restartBtn).toBeInTheDocument();
    expect(restartBtn.textContent.trim()).toBe("Restart");
    fireEvent.click(restartBtn);

    expect(container.querySelector("#result")).not.toBeInTheDocument();
    expect(text(container, "#questionNumber")).toBe("Question 1 of 3");
    expect(text(container, "#questionText")).toBe(Q1_TEXT);
    expect(text(container, "#score")).toBe("Score: 0");
    expect(container.querySelector("#feedback")).not.toBeInTheDocument();
    expect(container.querySelector("#nextBtn")).not.toBeInTheDocument();
    for (let i = 0; i < 4; i++) {
      expect(container.querySelector(`#option${i}`)).toBeEnabled();
    }
  });
});