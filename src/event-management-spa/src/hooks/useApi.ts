import { useQuery, useMutation, QueryKey, UseQueryOptions, UseMutationOptions } from '@tanstack/react-query';
import { AxiosError } from 'axios';

/**
 * Custom hook for making API GET requests with React Query
 * @param queryKey - The query key for React Query cache
 * @param queryFn - The function that makes the API request
 * @param options - Additional options for useQuery
 */
export function useApiQuery<TData, TError = AxiosError>(
  queryKey: QueryKey,
  queryFn: () => Promise<TData>,
  options?: Omit<UseQueryOptions<TData, TError, TData, QueryKey>, 'queryKey' | 'queryFn'>
) {
  return useQuery<TData, TError, TData, QueryKey>({
    queryKey,
    queryFn,
    ...options,
  });
}

/**
 * Custom hook for making API mutation requests (POST, PUT, DELETE) with React Query
 * @param mutationFn - The function that makes the API request
 * @param options - Additional options for useMutation
 */
export function useApiMutation<TData, TVariables, TError = AxiosError>(
  mutationFn: (variables: TVariables) => Promise<TData>,
  options?: Omit<UseMutationOptions<TData, TError, TVariables>, 'mutationFn'>
) {
  return useMutation<TData, TError, TVariables>({
    mutationFn,
    ...options,
  });
}
