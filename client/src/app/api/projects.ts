import axios from "axios";

const BASE_URL = "http://localhost:3001/api";

// API function that returns a list of all projects
export async function getAll() {
    const response = await fetch(`${BASE_URL}/projects`);
    if (!response.ok) {
        throw new Error('Network response was not ok');
    }
    return response.json();
}

// API function for adding time registration
export async function addTimeRegistration(projectId: number, timeSpent: number) {
    try {
        const response = await axios.post(`${BASE_URL}/projects/${projectId}/time-registration`, {
            timeSpent: timeSpent,
        });

        if (!response.status.toString().startsWith('2')) {
            throw new Error('Failed to add time registration');
        }

        return response.data; // return the API response data
    } catch (error) {
        console.error('Error adding time registration:', error);
        throw error;
    }
}
