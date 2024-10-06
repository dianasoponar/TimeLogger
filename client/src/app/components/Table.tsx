import React, { useEffect, useState } from "react";
import { getAll, addTimeRegistration } from "../api/projects";
import { IProject } from "../interfaces/project";
import { ITimeRegistration } from "../interfaces/timeregistration";
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import { faChevronDown, faChevronRight, faChevronUp } from '@fortawesome/free-solid-svg-icons';
import { ProjectStatusMap, TimeSpentMap } from '../api/enums';

export default function Table() {
    const [projects, setProjects] = useState<IProject[]>([]); // projects
    const [expandedProject, setExpandedProject] = useState<number | null>(null); // id of the expended project
    const [newTimeSpent, setNewTimeSpent] = useState<number>(0); // time spent input
    const [isSubmitting, setIsSubmitting] = useState<boolean>(false); // check if submission is in progress
    const [isAscending, setIsAscending] = useState(true); // state of deadline sorting

    useEffect(() => {
        const fetchProjects = async () => {
            try {
                const data = await getAll();

                // sort projects by deadline right after fetching
                const sortedProjects = sortProjectsByDeadline(data, isAscending);
                setProjects(sortedProjects);
            } catch (error) {
                console.error("Failed to fetch projects:", error);
            }
        };

        fetchProjects();
    }, [isAscending]);

    // function to sort projects by deadline
    const sortProjectsByDeadline = (data: IProject[], ascending: boolean) => {
        return data.sort((a: IProject, b: IProject) => {
            const dateA = a.deadline ? new Date(a.deadline).getTime() : 0;
            const dateB = b.deadline ? new Date(b.deadline).getTime() : 0;
            return ascending ? dateA - dateB : dateB - dateA;
        });
    };

    // sort projects by deadline
    const sortByDeadline = () => {
        const sorted = sortProjectsByDeadline(projects, isAscending);

        setProjects(sorted);
        setIsAscending(!isAscending);
    };

    // set action for project selection
    const toggleExpandProject = (index: number) => {
        setExpandedProject(expandedProject === index ? null : index);
    };

    // handle new time registrations
    const handleAddTimeRegistration = async (projectId: number) => {
        if (newTimeSpent === 0) return; // Avoid empty submissions

        setIsSubmitting(true);

        try {
            await addTimeRegistration(projectId, newTimeSpent);

            setNewTimeSpent(0); // clear input after submission

            const updatedProjects = await getAll(); // get all projects
            const sorted = sortProjectsByDeadline(updatedProjects, isAscending);
            setProjects(sorted);
        } catch (error) {
            console.error("Error adding time registration:", error);
        } finally {
            setIsSubmitting(false);
        }
    };

    return (
        <div>
            <table className="w-full">
                <thead className="bg-gray-200">
                    <tr>
                        <th className="py-2 text-left">Project Name</th>
                        <th className="py-2 text-left">Status</th>
                        <th
                            className="py-2 text-left cursor-pointer"
                            onClick={sortByDeadline}
                        >
                            Deadline
                            <span className="ml-2">
                                <FontAwesomeIcon
                                    icon={isAscending ? faChevronUp : faChevronDown}
                                />
                            </span>
                        </th>
                    </tr>
                </thead>
                <tbody>
                    {projects.map((project: IProject) => (
                        <React.Fragment key={project.id}>
                            <tr onClick={() => toggleExpandProject(project.id)} className="cursor-pointer hover:bg-gray-100">
                                <td className="py-2 flex items-center">
                                    <span className="mr-2">
                                        <FontAwesomeIcon icon={expandedProject === project.id ? faChevronDown : faChevronRight} />
                                    </span>
                                    {project.name}
                                </td>
                                <td className="py-2">{ProjectStatusMap[project.status]}</td>
                                <td className="py-2">{project.deadline ? new Date(project.deadline).toISOString().split('T')[0] : "N/A"}</td>
                            </tr>
                            {expandedProject === project.id && (
                                <tr>
                                    <td colSpan={3}>
                                        <table className="w-full mt-2 bg-gray-100">
                                            <thead className="bg-gray-300">
                                                <tr>
                                                    <th className="py-1 text-left">Date Registered</th>
                                                    <th className="py-1 text-left">Time Registered</th>
                                                </tr>
                                            </thead>
                                            <tbody>
                                                <>
                                                    {project.timeRegistrations.map((timeReg: ITimeRegistration, index: number) => (
                                                        <tr key={index}>
                                                            <td className="py-1">{new Date(timeReg.registrationDate).toISOString().split('T')[0]}</td>
                                                            <td className="py-1">{TimeSpentMap[timeReg.timeSpent]}</td>
                                                        </tr>
                                                    ))}
                                                    {/* Add new time registration row */}
                                                    {
                                                        project.status != 2 && (<tr>
                                                            <td className="py-2">
                                                                <input
                                                                    type="text"
                                                                    value=""
                                                                    disabled
                                                                    className="w-full bg-gray-200 py-1 px-2"
                                                                />
                                                            </td>
                                                            <td className="py-2">
                                                                <select
                                                                    value={newTimeSpent}
                                                                    onChange={(e) => setNewTimeSpent(Number(e.target.value))}
                                                                    className="w-full bg-white py-1 px-2 border"
                                                                >
                                                                    {Object.entries(TimeSpentMap).map(([value, label]) => (
                                                                        <option key={value} value={value}>
                                                                            {label}
                                                                        </option>
                                                                    ))}
                                                                </select>
                                                            </td>
                                                            <td className="py-2">
                                                                <button
                                                                    onClick={() => handleAddTimeRegistration(project.id)}
                                                                    className="px-4 py-1 bg-blue-500 text-white rounded disabled:bg-blue-300"
                                                                    disabled={isSubmitting}
                                                                >
                                                                    {isSubmitting ? "Submitting..." : "Add Time"}
                                                                </button>
                                                            </td>
                                                        </tr>)
                                                    }
                                                </>
                                            </tbody>
                                        </table>
                                    </td>
                                </tr>
                            )}
                        </React.Fragment>
                    ))}
                </tbody>
            </table>
        </div>
    );
}
