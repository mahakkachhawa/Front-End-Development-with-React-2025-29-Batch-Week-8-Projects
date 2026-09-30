import React from "react";

const GIFT_LIMIT = 50;

// CLASS COMPONENT: state is managed with this.state / this.setState
class GiftMessage extends React.Component {
  constructor(props) {
    super(props);

    this.state = {
      gift: "",
      savedGift: "",
      error: "",
    };

    // Bind class event handlers so `this` points to the component
    this.handleGiftChange = this.handleGiftChange.bind(this);
    this.handleSaveGift = this.handleSaveGift.bind(this);
  }

  handleGiftChange(event) {
    this.setState({ gift: event.target.value });
  }

  handleSaveGift() {
    const trimmedGift = this.state.gift.trim();

    if (trimmedGift === "") {
      this.setState({ error: "Please write a message" });
    } else {
      this.setState({ savedGift: trimmedGift, error: "" });
    }
  }

  render() {
    const { gift, savedGift, error } = this.state;

    return (
      <section className="section">
        <p id="classLabel" className="badge badge-class">Class Component (this.state)</p>

        <h2 className="section-heading">Add a gift message</h2>

        <input
          id="giftInput"
          className={error ? "text-input input-error" : "text-input"}
          type="text"
          placeholder="Write your gift message"
          value={gift}
          onChange={this.handleGiftChange}
        />
        <p
          id="giftCounter"
          className={gift.length > GIFT_LIMIT ? "counter counter-over" : "counter"}
        >
          {`${gift.length} / ${GIFT_LIMIT}`}
        </p>

        <button id="saveGiftBtn" className="save-btn" onClick={this.handleSaveGift}>
          Save Gift Message
        </button>

        {error && <p id="giftError" className="message message-error">{error}</p>}
        {savedGift && (
          <p id="savedGift" className="message message-ok">{`Gift message: ${savedGift}`}</p>
        )}
      </section>
    );
  }
}

export default GiftMessage;