import pandas as pd
from sklearn.feature_extraction.text import TfidfVectorizer
from sklearn.metrics.pairwise import linear_kernel
import json
import numpy as np
import argparse
import sys

def deep_convert_numpy(obj):
    if isinstance(obj, np.generic):
        return obj.item()
    elif isinstance(obj, dict):
        return {deep_convert_numpy(key): deep_convert_numpy(value) for key, value in obj.items()}
    elif isinstance(obj, list):
        return [deep_convert_numpy(item) for item in obj]
    elif isinstance(obj, tuple):
        return tuple(deep_convert_numpy(item) for item in obj)
    else:
        return obj



def preprocess_and_cache(df, col_weights, max_returned, threshold, output, verbose = False):
    # Initialize TfidfVectorizer with English stop words
    tfidf = TfidfVectorizer(stop_words='english')

    # Dictionary to store the cosine similarity matrices for each column
    cosine_similarities = {}

    # Compute and store TF-IDF matrices and their cosine similarities
    for col in col_weights.keys():
        # Create a TF-IDF matrix for the column
        tfidf_matrix = tfidf.fit_transform(df[col].fillna(''))
        
        # Calculate the cosine similarity matrix for this column
        cosine_sim = linear_kernel(tfidf_matrix, tfidf_matrix)
        
        # Store the cosine similarity matrix in the dictionary
        cosine_similarities[col] = cosine_sim

    # Initialize a matrix to hold the combined weighted similarities
    cosine_sim = np.zeros_like(cosine_similarities[list(col_weights.keys())[0]])

    # Weight and combine the cosine similarity matrices
    for col, weight in col_weights.items():
        cosine_sim += cosine_similarities[col] * weight
        
    similarities = {}

    # Generate the similarities for each item
    for idx, row in enumerate(cosine_sim):
        if verbose:
            print(f"Processing item {idx + 1} of {len(cosine_sim)}", file=sys.stderr)
        
        if row.size < max_returned:
            # Adjust max_returned or handle the small array case differently
            max_returned = row.size  # Simplest solution, but adjust based on your needs

        # Get indices of top 20 scores without sorting all scores
        top_indices = np.argpartition(row, -max_returned)[-max_returned:]
        
        # top_scores = row[top_indices]
        
        current_item_id = df.iloc[idx]['Id']
        # # Filter out self-similarity and scores below 0.1
        
        items_scores = [(item_id, score, idx) for idx, (item_id, score) in enumerate(zip(df['Id'], row))]

        # Filter out self-similarity and scores below threshold in a more efficient way
        relevant_scores_with_row_id = [
            (item_id, score) for item_id, score, idx in items_scores
            if item_id != current_item_id and score > threshold and idx in top_indices
        ]

        # Sort the filtered list
        sorted_relevant_scores = sorted(relevant_scores_with_row_id, key=lambda x: x[1], reverse=True)
        ## keep only the last 6 digits of similarity score
        sorted_relevant_scores = [(item_id, round(score, 6)) for item_id, score in sorted_relevant_scores]
        
            
        similarities[current_item_id] = {similar_id: score for similar_id, score in sorted_relevant_scores}
        
        
        
        
        
    
    # Convert the similarities to a serializable format
    similarities_serializable = deep_convert_numpy(similarities)
    
    
    try:
        with open(output, 'w') as f:
            json.dump(similarities_serializable, f, indent=4)
    except Exception as e:
        print(e, file=sys.stderr)
        exit(1)

if __name__ == "__main__":
    parser = argparse.ArgumentParser(description="Process dataset for similarity analysis.")
    parser.add_argument("--input", required=True, help="Path to the input CSV file")
    parser.add_argument("--col-weights", nargs='+', required=True, help="Column indices and their weights in format 'index:weight'")
    parser.add_argument("--max-returned", type=int, default=80, help="Maximum number of similar items to return")
    parser.add_argument("--threshold", type=float, default=0.005, help="Threshold for similarity")
    parser.add_argument("--output", default="similarities.json", help="Path to the output JSON file")
    parser.add_argument("--verbose", default=False, action='store_true', help="Print verbose output")
    

    args = parser.parse_args()

    col_weights = {string.split(':')[0]: float(string.split(':')[1]) for string in args.col_weights}
    
    columns_to_use = list(col_weights.keys()) + ['Id']
    
    try:
        df = pd.read_csv(args.input, usecols=columns_to_use, low_memory=False)
    except ValueError as e:
        print("Invalid column index.", e, file=sys.stderr)
        exit(2)
    except FileNotFoundError:
        print("File not found.", file=sys.stderr)
        exit(3)
    except Exception as e:
        print(e, file=sys.stderr)
        exit(1)
    
    preprocess_and_cache(df, col_weights, args.max_returned, args.threshold, args.output, args.verbose)