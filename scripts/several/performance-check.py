import requests
import json

# Simple script to add a lot of stubs for performance checking.
def add_stubs():
    for i in range(0,100000):
        request = {
            'id': 'request-{}'.format(i),
            'conditions': {
                'method': 'POST',
                'url': {
                    'path': '/post-{}'.format(i)
                },
                'body': ['^post-val-{}$'.format(i)]
            },
            'response': {
                'text': 'OK{}!'.format(i)
            }
        }
        
        result = requests.post('http://localhost:5000/ph-api/stubs', json=request)
        print('Result of request {}: {}'.format(i, result.status_code))

add_stubs()