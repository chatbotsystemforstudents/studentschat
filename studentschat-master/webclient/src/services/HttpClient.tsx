export function SendToApi(request: any, apiUrl: string): Promise<any> {
	return fetch(apiUrl, {
		method: 'POST',
		headers: {
			'accept': '*/*',
			'Content-Type': 'application/json', // Optional, depending on API requirements
		},
		body: JSON.stringify(request), // Convert the request JSON object to a string
	})
	.then((response) => {
		if (!response.ok) {
			throw new Error(`HTTP error! Status: ${response.status}`);
		}
		// Check if the response body is empty
		return response.text().then((text) => {
			return text ? JSON.parse(text) : {}; // Parse JSON or return an empty object
		});
	});
}
export function PollUrl(url: string): Promise<boolean> {
	return new Promise((resolve, reject) => {
		let intervalCounter: number=0;
		let interval: number;
		function checkUrl(url: string): void {
			if (intervalCounter > 30)
				reject(new Error(`link seems unavailable.Max polling counter reached:${intervalCounter}`))
			fetch(url)
				.then(response => {
					if (response.status === 200) {
						clearInterval(interval);
						resolve(true); // URL exists, resolve the Promise
					} else if (response.status === 404) {
						// Handle 404 silently without logging to console
						// Continue polling
						intervalCounter++;
						return;
					} else {
						clearInterval(interval);
						reject(new Error(`Unexpected response status: ${response.status}`));
					}
				})
				.catch(error => {
					clearInterval(interval);
					reject(error); // Reject the Promise on fetch error
				});
		}

		interval = setInterval(() => {
			checkUrl(url);
		}, 450);		
	});
}

