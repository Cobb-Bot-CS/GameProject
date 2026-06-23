from flask import Flask, render_template, request, jsonify
import requests
from datetime import datetime, timedelta

app = Flask(__name__)

COINGECKO_API_BASE = "https://api.coingecko.com/api/v3"

def get_coin_data(coin_id):
    """Fetch coin data from CoinGecko API"""
    try:
        # Get basic coin info
        url = f"{COINGECKO_API_BASE}/coins/{coin_id}"
        params = {
            'localization': 'false',
            'tickers': 'false',
            'community_data': 'false',
            'developer_data': 'false'
        }
        response = requests.get(url, params=params, timeout=10)
        
        if response.status_code == 404:
            return None, "Cryptocurrency not found. Please check the name and try again."
        
        response.raise_for_status()
        data = response.json()
        
        # Get 7-day market chart data
        chart_url = f"{COINGECKO_API_BASE}/coins/{coin_id}/market_chart"
        chart_params = {
            'vs_currency': 'usd',
            'days': '7',
            'interval': 'daily'
        }
        chart_response = requests.get(chart_url, params=chart_params, timeout=10)
        chart_response.raise_for_status()
        chart_data = chart_response.json()
        
        # Extract relevant information
        coin_info = {
            'name': data.get('name', 'N/A'),
            'symbol': data.get('symbol', 'N/A').upper(),
            'image': data.get('image', {}).get('large', ''),
            'current_price': data.get('market_data', {}).get('current_price', {}).get('usd', 'N/A'),
            'market_cap_rank': data.get('market_cap_rank', 'N/A'),
            'market_cap': data.get('market_data', {}).get('market_cap', {}).get('usd', 'N/A'),
            'price_change_24h': data.get('market_data', {}).get('price_change_percentage_24h', 'N/A'),
            'description': data.get('description', {}).get('en', 'No description available.'),
            'homepage': data.get('links', {}).get('homepage', [''])[0],
            'chart_labels': [],
            'chart_prices': []
        }
        
        # Process chart data
        if 'prices' in chart_data:
            for price_point in chart_data['prices']:
                timestamp = price_point[0]
                price = price_point[1]
                date = datetime.fromtimestamp(timestamp / 1000).strftime('%m/%d')
                coin_info['chart_labels'].append(date)
                coin_info['chart_prices'].append(round(price, 2))
        
        return coin_info, None
        
    except requests.exceptions.Timeout:
        return None, "Request timed out. Please try again."
    except requests.exceptions.RequestException as e:
        return None, f"Error fetching data: {str(e)}"
    except Exception as e:
        return None, f"An unexpected error occurred: {str(e)}"

@app.route('/', methods=['GET', 'POST'])
def index():
    coin_info = None
    error = None
    
    if request.method == 'POST':
        coin_id = request.form.get('coin_id', '').strip().lower()
        
        if not coin_id:
            error = "Please enter a cryptocurrency name."
        else:
            coin_info, error = get_coin_data(coin_id)
    
    return render_template('index.html', coin_info=coin_info, error=error)

if __name__ == '__main__':
    app.run(debug=True)