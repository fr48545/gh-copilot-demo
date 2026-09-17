/**
 * Validates a date entered in French format (DD/MM/YYYY) and converts it to a
 * JavaScript Date instance.
 *
 * Rules:
 * - the value must strictly match the pattern dd/mm/yyyy
 * - the date must be a real calendar date (for example, 31/02/2025 is rejected)
 * - invalid values or malformed input return null
 *
 * Example:
 *   validateDate("25/03/2025") -> Date(2025, 2, 25)
 *   validateDate("31/02/2025") -> null
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
 * Validates a GUID using the canonical UUID format defined by RFC 4122.
 *
 * Accepted pattern:
 *   xxxxxxxx-xxxx-4xxx-8xxx-xxxxxxxxxxxx
 * with hexadecimal digits, case-insensitive.
 *
 * Example:
 *   validateGuid("550e8400-e29b-41d4-a716-446655440000") -> true
 *   validateGuid("not-a-guid") -> false
 */
export function validateGuid(value: string): boolean {
	return /^[0-9a-f]{8}-[0-9a-f]{4}-[1-5][0-9a-f]{3}-[89ab][0-9a-f]{3}-[0-9a-f]{12}$/i.test(
		value.trim(),
	);
}

/**
 * Validates an IPv6 address string, including standard compressed notation and
 * IPv4-mapped forms such as ::ffff:192.168.0.1.
 *
 * The function rejects:
 * - empty strings
 * - strings containing spaces
 * - addresses with more than one "::" compression sequence
 * - invalid hexadecimal groups or overlong IPv6 groups
 * - invalid IPv4 dotted-quad tails
 *
 * Example:
 *   validateIPV6("2001:db8::1") -> true
 *   validateIPV6("::ffff:192.168.0.1") -> true
 *   validateIPV6("2001:db8:::1") -> false
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

