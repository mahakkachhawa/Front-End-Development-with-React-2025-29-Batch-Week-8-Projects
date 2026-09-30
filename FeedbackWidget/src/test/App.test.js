import React from "react";
import { render, fireEvent } from "@testing-library/react";
import "@testing-library/jest-dom";
import App from "../App";

const selectRating = (container, n) => {
  fireEvent.click(container.querySelector(`#star${n}`));
};

const typeComment = (container, value) => {
  fireEvent.change(container.querySelector("#feedbackInput"), { target: { value } });
};

describe("Feedback Widget App", () => {
  test("verify_heading_rendering", () => {
    const { container } = render(<App />);
    const title = container.querySelector("h1#title");
    expect(title).toBeInTheDocument();
    expect(title).toHaveTextContent("Feedback Widget");
  });

  test("verify_five_rating_buttons_rendering", () => {
    const { container } = render(<App />);
    for (let n = 1; n <= 5; n++) {
      const btn = container.querySelector(`button#star${n}`);
      expect(btn).toBeInTheDocument();
      expect(btn.textContent.trim()).toBe(String(n));
    }
  });

  test("verify_initial_state_of_form", () => {
    const { container } = render(<App />);
    expect(container.querySelector("p#selectedRating")).toHaveTextContent("No rating selected");
    expect(container.querySelector("#ratingMessage")).not.toBeInTheDocument();
    expect(container.querySelector("textarea#feedbackInput").value).toBe("");
    expect(container.querySelector("#thankYou")).not.toBeInTheDocument();
  });

  test("verify_submit_button_is_disabled_until_rating_selected", () => {
    const { container } = render(<App />);
    const submitBtn = container.querySelector("button#submitBtn");
    expect(submitBtn).toBeInTheDocument();
    expect(submitBtn.textContent.trim()).toBe("Submit");
    expect(submitBtn).toBeDisabled();

    selectRating(container, 3);
    expect(container.querySelector("#submitBtn")).toBeEnabled();
  });

  test("verify_selected_rating_text_updates_on_click", () => {
    const { container } = render(<App />);
    selectRating(container, 4);
    expect(container.querySelector("#selectedRating").textContent).toBe("Selected rating: 4");
    selectRating(container, 2);
    expect(container.querySelector("#selectedRating").textContent).toBe("Selected rating: 2");
  });

  test("verify_rating_message_for_each_rating", () => {
    const { container } = render(<App />);
    const expected = {
      1: "We will try to do better",
      2: "We will try to do better",
      3: "Thanks for the feedback",
      4: "Glad you liked it!",
      5: "Glad you liked it!",
    };
    Object.keys(expected).forEach((n) => {
      selectRating(container, n);
      const message = container.querySelector("p#ratingMessage");
      expect(message).toBeInTheDocument();
      expect(message.textContent).toBe(expected[n]);
    });
  });

  test("verify_submit_shows_thank_you_and_hides_form", () => {
    const { container } = render(<App />);
    selectRating(container, 4);
    typeComment(container, "Great session");
    fireEvent.click(container.querySelector("#submitBtn"));

    const thankYou = container.querySelector("div#thankYou");
    expect(thankYou).toBeInTheDocument();
    expect(thankYou.textContent).toBe("Thank you for your 4-star feedback!");

    for (let n = 1; n <= 5; n++) {
      expect(container.querySelector(`#star${n}`)).not.toBeInTheDocument();
    }
    expect(container.querySelector("#feedbackInput")).not.toBeInTheDocument();
    expect(container.querySelector("#submitBtn")).not.toBeInTheDocument();
  });

  test("verify_submitted_comment_is_shown_when_entered", () => {
    const { container } = render(<App />);
    selectRating(container, 5);
    typeComment(container, "  Very helpful workshop  ");
    fireEvent.click(container.querySelector("#submitBtn"));

    const submitted = container.querySelector("p#submittedComment");
    expect(submitted).toBeInTheDocument();
    expect(submitted.textContent).toBe("Your comment: Very helpful workshop");
  });

  test("verify_submitted_comment_is_absent_when_no_comment", () => {
    const { container } = render(<App />);
    selectRating(container, 2);
    fireEvent.click(container.querySelector("#submitBtn"));
    expect(container.querySelector("#thankYou").textContent).toBe(
      "Thank you for your 2-star feedback!"
    );
    expect(container.querySelector("#submittedComment")).not.toBeInTheDocument();

    // A whitespace-only comment counts as "no comment"
    fireEvent.click(container.querySelector("#newFeedbackBtn"));
    selectRating(container, 3);
    typeComment(container, "     ");
    fireEvent.click(container.querySelector("#submitBtn"));
    expect(container.querySelector("#submittedComment")).not.toBeInTheDocument();
  });

  test("verify_give_new_feedback_resets_the_form", () => {
    const { container } = render(<App />);
    selectRating(container, 5);
    typeComment(container, "Loved it");
    fireEvent.click(container.querySelector("#submitBtn"));

    const newBtn = container.querySelector("button#newFeedbackBtn");
    expect(newBtn).toBeInTheDocument();
    expect(newBtn.textContent.trim()).toBe("Give New Feedback");
    fireEvent.click(newBtn);

    expect(container.querySelector("#thankYou")).not.toBeInTheDocument();
    expect(container.querySelector("#newFeedbackBtn")).not.toBeInTheDocument();
    expect(container.querySelector("#star1")).toBeInTheDocument();
    expect(container.querySelector("#star5")).toBeInTheDocument();
    expect(container.querySelector("#selectedRating")).toHaveTextContent("No rating selected");
    expect(container.querySelector("#ratingMessage")).not.toBeInTheDocument();
    expect(container.querySelector("#feedbackInput").value).toBe("");
    expect(container.querySelector("#submitBtn")).toBeDisabled();
  });
});