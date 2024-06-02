// import { useState } from 'react';
import "../../App.css";
import bgImage from "../../assets/collage5.jpg";

function Background({banner}) {
  if(banner){
    return (
      <div className="bg-container">
          <img src={banner} alt="" className="bg-image" />
        </div>
    );
  }
  return (
        <div className="bg-container">
          <img src={bgImage} alt="" className="bg-image" />
        </div>
  );
}

export default Background;
