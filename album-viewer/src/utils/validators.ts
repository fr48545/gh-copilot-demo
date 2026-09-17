/**
 * Validates a date entered in French format (DD/MM/YYYY) and converts it to a
 * Date. Invalid or incorrectly formatted values return null.
 */
export function validateDate(value: string): Date | null {
	const match = /^(\d{2})\/(\d{2})\/(\d{4})$/.exec(value.trim());

	if (!match) {
		return null;
	}

	const day = Number(match[1]);
	const month = Number(match[2]);
	const year = Number(match[3]);
	const date = new Date(year, month - 1, day);

	if (
		date.getFullYear() !== year ||
		date.getMonth() !== month - 1 ||
		date.getDate() !== day
	) {
		return null;
	}

	return date;
}

/**
 * Validates a GUID string in canonical format.
 */
export function validateGuid(value: string): boolean {
	return /^[0-9a-f]{8}-[0-9a-f]{4}-[1-5][0-9a-f]{3}-[89ab][0-9a-f]{3}-[0-9a-f]{12}$/i.test(
		value.trim(),
	);
}

/**
 * Validates an IPv6 address string, including compressed and IPv4-mapped forms.
 */
export function validateIPV6(value: string): boolean {
	const address = value.trim();

	if (!address || address.includes(" ") || address.split("::").length > 2) {
		return false;
	}

	const [head, tail] = address.split("::");
	const parseGroups = (part: string): string[] | null => {
		if (!part) {
			return [];
		}

		const groups = part.split(":");
		if (groups.some((group) => !group || !/^[0-9a-f]+$/i.test(group))) {
			return null;
		}

		const last = groups[groups.length - 1];
		if (last.includes(".")) {
			const octets = last.split(".");
			if (
				octets.length !== 4 ||
				octets.some(
					(octet) =>
						!/^(?:0|[1-9]\d{0,2})$/.test(octet) || Number(octet) > 255,
				)
			) {
				return null;
			}

			groups[groups.length - 1] = "ipv4";
		}

		if (groups.some((group) => group !== "ipv4" && group.length > 4)) {
			return null;
		}

		return groups;
	};

	const headGroups = parseGroups(head);
	const tailGroups = parseGroups(tail);
	if (!headGroups || !tailGroups) {
		return false;
	}

	const groupCount = (groups: string[]) =>
		groups.reduce((count, group) => count + (group === "ipv4" ? 2 : 1), 0);

	return address.includes("::")
		? groupCount(headGroups) + groupCount(tailGroups) < 8
		: groupCount(headGroups) === 8;
}

