import { Button, Card, Image, Space, Typography } from "antd";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import {
  faCalendarDays,
  faStar,
  faArrowRotateRight,
  faDice
} from "@fortawesome/free-solid-svg-icons";


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
  refreshPick
}) {

  const buttonHandler = () => {
    refreshPick();
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
      </div>
      <Button onClick={buttonHandler} className="find-btn" style={{position: "absolute", bottom: "0px", right: "0px", margin: "10px", fontWeight: "600", fontSize: "15px", height: "fit-content"}}><FontAwesomeIcon icon={faDice} style={{marginRight: "7px", fontSize: "20px"}}/>Refresh<FontAwesomeIcon icon={faDice} style={{marginLeft: "7px", fontSize: "20px"}}/></Button>
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
          <Typography.Title level={4} style={{ textTransform: "capitalize", marginTop: "10px", marginBottom: "0px", color: "white" }}>
            {title}
          </Typography.Title>
        </Space>
      </Card>
    );
  }
  

}

export default MovieCard;
