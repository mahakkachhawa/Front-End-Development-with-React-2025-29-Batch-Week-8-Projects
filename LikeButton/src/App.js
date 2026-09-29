import React from "react";

// Requirement 1: a React class component (no useState anywhere)
class App extends React.Component {
  constructor(props) {
    super(props);

    // Requirement 4: state lives in this.state
    this.state = {
      likeCount: 0,
      isLiked: false,
    };

    // Bind the class event handler so `this` points to the component
    this.handleLikeClick = this.handleLikeClick.bind(this);
  }

  // Requirement 10: class event handler method + functional form of setState
  handleLikeClick() {
    this.setState((prevState) => ({
      likeCount: prevState.isLiked ? prevState.likeCount - 1 : prevState.likeCount + 1,
      isLiked: !prevState.isLiked,
    }));
  }

  render() {
    const { likeCount, isLiked } = this.state;

    return (
      <div className="page">
        <div className="post-card">
          <h1 id="title" className="post-title">Class Like Button</h1>
          <p id="componentType" className="component-type">Class Component</p>

          <p className="post-text">
            Just finished my first React project. Learning components, props and state
            has been a great journey so far!
          </p>

          <div className="like-row">
            <span className={isLiked ? "heart heart-active" : "heart"} aria-hidden="true">
              ♥
            </span>
            <span id="likeCount" className="like-count">{likeCount}</span>
            <span className="like-suffix">likes</span>
          </div>

          <button
            id="likeBtn"
            className={isLiked ? "like-btn like-btn-active" : "like-btn"}
            onClick={this.handleLikeClick}
          >
            {isLiked ? "Unlike" : "Like"}
          </button>

          <p
            id="likeStatus"
            className={isLiked ? "like-status status-liked" : "like-status"}
          >
            {isLiked ? "You liked this post" : "You have not liked this post"}
          </p>
        </div>
      </div>
    );
  }
}

export default App;