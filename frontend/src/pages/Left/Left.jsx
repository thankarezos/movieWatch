import { Button } from "antd";
import { useEffect } from 'react';
import MovieCard from '../MovieCard/MovieCard';
import PropTypes from "prop-types";

const Left = ({ checkedMoviesArray = [], toggleMovieCheck, buttonHandler }) => {

    useEffect(() => {
        console.log('mpike');
        console.log(checkedMoviesArray);
    }, [checkedMoviesArray]);

    return (
        <div style={{ backgroundColor: "rgba(0, 0, 0, 0.9)", width: "280px", height: "700px", alignContent: "start", zIndex: 10, position: "fixed", left: "0px", transform: "translate(50%)", top: "150px", display: "flex", flexFlow: "row wrap", borderRadius: "10px", padding: "15px", boxShadow: "2px 2px 2px 2px black", marginRight: "20px" }}>
            {checkedMoviesArray.map(movie => (
                <div
                    key={movie.id}
                    onClick={() => toggleMovieCheck(movie.id)}
                    style={{ margin: '5px' }}
                >
                    <MovieCard title={movie.title} poster={movie.poster} size={100} />
                </div>
            ))}
            <Button className='find-btn' style={{ position: "absolute", bottom: "-50px", right: "0" }} onClick={buttonHandler}>Find Movies</Button>
        </div>
    );
}

Left.propTypes = {
    checkedMoviesArray: PropTypes.array,
    toggleMovieCheck: PropTypes.func.isRequired,
    buttonHandler: PropTypes.func.isRequired
};

export default Left;
