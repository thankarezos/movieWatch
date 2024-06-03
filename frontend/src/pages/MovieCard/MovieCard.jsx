import { Button, Card, Image, Space, Typography } from "antd";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import {
  faCalendarDays,
  faStar,
  faBookmark,
  faDice,
  faClapperboard
} from "@fortawesome/free-solid-svg-icons";
import { faYoutube } from '@fortawesome/free-brands-svg-icons';
import { faBookmark as farBookmark } from '@fortawesome/free-regular-svg-icons';
import Profile from "../Profile/Profile"
import { useEffect, useState } from "react";


function MovieCard({
  poster,
  title,
  rating,
  genre,
  year,
  desc,
  link,
  size,
  isOverview,
  refreshPick,
  noRefresh,
}) {

  const [isFave, setIsFave] = useState(false);

  const buttonHandler = () => {
    refreshPick();
  }

  const dontRefresh = !!noRefresh;

  const handleFavButton = () => {
    setIsFave(!isFave);
  }

  if(isOverview){
    return(
      <Card
    style={{
      backgroundColor: "rgba(255, 255, 255, 0)",
      border: "0px solid",
    }}
  >
    <Space style={{ display: "flex", flexDirection: "row", margin: "-10px" }}>
      <div>
        <Image
          src={poster}
          width={size ? size : 385}
          preview={false}
        />
      </div>
      <div style={{height: "560px", marginLeft: "20px", display: "flex", flexDirection: "column", justifyContent: "start", alignItems: "flex-start", textAlign: "left"}}>
        <Typography.Title level={1} style={{ textTransform: "capitalize", marginTop: "10px", marginBottom: "20px", color: "white" }}>
          {title}
        </Typography.Title>
        <div style={{width: "300px", display: "flex", flexDirection: "row", justifyContent: "space-between"}}>
          <Typography.Title level={3} style={{color: "white", margin: "0px 0px 20px 0px"}}><FontAwesomeIcon icon={faCalendarDays} style={{marginRight: "10px"}}/>{year}</Typography.Title>
          <Typography.Title level={3} style={{color: "white", margin: "0px 0px 20px 0px"}}><FontAwesomeIcon icon={faStar} style={{marginRight: "10px", color: "#FFD700"}}/>{rating}/10</Typography.Title>
        </div>
        <Typography.Title level={4} style={{color: "white", textTransform: "capitalize", margin: "0px 0px 20px 0px"}}>Genre: {genre.join(', ')}</Typography.Title>
        <Typography.Text style={{color: "white", fontSize: "18px"}}>{desc}</Typography.Text>
        <a href={link} target="_blank" rel="noreferrer" style={{fontSize: "20px", marginTop: "20px"}}><FontAwesomeIcon icon={faClapperboard} style={{marginRight: "5px"}}/> Watch Trailer</a>
      </div>
      <Button onClick={buttonHandler} className="find-btn" style={{position: "absolute", bottom: "0px", right: "0px", margin: "10px", fontWeight: "600", fontSize: "15px", height: "fit-content", display: dontRefresh ? "none" : "block"}}><FontAwesomeIcon icon={faDice} style={{marginRight: "7px", fontSize: "20px"}}/>Refresh<FontAwesomeIcon icon={faDice} style={{marginLeft: "7px", fontSize: "20px"}}/></Button>
      <FontAwesomeIcon icon={isFave ? faBookmark : farBookmark} className="fav-btn" onClick={() => handleFavButton()}/>
    </Space>
  </Card>
    );
  }
  else {
    return (
      <Card
        style={{
          backgroundColor: "rgba(255, 255, 255, 0)",
          border: "0px solid",
        }}
      >
        <Space style={{ display: "flex", flexDirection: "column", margin: "-10px" }}>
          <div>
            <Image
              src={poster}
              width={size ? size : 160}
              preview={false}
            />
          </div>
          <Typography.Title level={4} style={{ textTransform: "capitalize", marginTop: "10px", marginBottom: "0px", color: "white", textWrap: "wrap", maxWidth: "160px" }}>
            {title}
          </Typography.Title>
        </Space>
      </Card>
    );
  }
  

}


MovieCard.propTypes = {
  poster: String,
  title: String,
  rating: String,
  genre: Array,
  year: String,
  desc: String,
  link: String,
  size: Number,
  isOverview: Boolean,
  refreshPick: Function,
  noRefresh: Boolean,
};
export default MovieCard;
