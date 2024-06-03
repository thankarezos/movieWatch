import { useState } from "react";
import "../../App.css";
import { Flex, Spin } from "antd";
import Background from "../Background/Background";
import MovieCard from "../MovieCard/MovieCard";
import PropTypes from "prop-types";
import apiService from "../../ApiService";
import { useNavigate } from "react-router-dom";

function Results({
  id,
  poster,
  title,
  banner,
  rating,
  genre,
  year,
  desc,
  refreshPick,
  noRefresh,
}) {
  const [isVisible] = useState(true); // Make this true when the results are ready
  const [trailer, setTrailer] = useState(null);
  const navigate = useNavigate();

  const getTrailer = async () => {
    const response = await apiService.get("/Movies/Trailers/" + id);

    console.log(response.data.data[0]);

    setTrailer(response.data.data[0]);
    navigate(trailer);
  }

  // const refreshPick = () => {
  //   console.log("Refreshed!");
  // }

  const dontRefresh = !!noRefresh;

  return (
    <div>
      <Background banner={banner} />
      <Spin
        size="large"
        style={{
          backgroundColor: "rgba(0,0,0,0.8)",
          padding: "15px",
          borderRadius: "50px",
          zIndex: "-100",
          position: "absolute",
          display: isVisible ? "none" : "block",
        }}
      />
      <Flex align="center" gap="middle" style={{ position: "relative" }}>
        <div
          style={{
            backgroundColor: "rgba(0, 0, 0, 0.9)",
            width: "1250px",
            height: "600px",
            display: isVisible ? "flex" : "none",
            flexFlow: "row wrap",
            borderRadius: "10px",
            padding: "15px",
            boxShadow: "2px 2px 2px 2px black",
          }}
        >
          <MovieCard
            noRefresh={dontRefresh}
            refreshPick={refreshPick}
            isOverview={true}
            title={title}
            year={year}
            rating={rating}
            genre={genre}
            poster={poster}
            desc={
              desc
            }
            onClick={getTrailer}
          />
        </div>
      </Flex>
    </div>
  );
}

Results.propTypes = {
  id: PropTypes.number,
  poster: PropTypes.string,
  banner: PropTypes.string,
  title: PropTypes.string,
  rating: PropTypes.number,
  genre: PropTypes.array,
  year: PropTypes.number,
  desc: PropTypes.string,
  link: PropTypes.string,
  size: PropTypes.number,
  isOverview: PropTypes.bool,
  refreshPick: PropTypes.func,
  noRefresh: PropTypes.bool,
};

export default Results;
